using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pooling;
using Gameplay.Aggro;
using Gameplay.Cards;
using Gameplay.Dice;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using UI.Interaction;
using UI.Browsers;
using UnityEngine;
using Util;
using Util.Interaction;
using Util.Patterns;
using Random = UnityEngine.Random;

namespace Gameplay.Tokens
{
    public abstract class ControllableToken<T> : Token<T, ControllableAggroManager>,
        IControllableToken, IInteractableOnDrag
        where T : Scriptable.Token
    {
        public ControllableAggroManager AggroManager => aggroManager;

        public override Vector4 OutlineColor => ActionPoints == 0
            ? GlobalDefinitions.TokenOutlineYellowColor
            : GlobalDefinitions.TokenOutlineGreenColor;

        public override bool CanInteract => TurnManager.CurrentStage is TurnStage.PlayersTurn && (CanAct || CanWalk);
        public override bool CanWalk => base.CanWalk || CanAct;

        protected abstract bool CanInteractWithCards { get; }


        
        protected override void Init()
        {
            base.Init();
            PoolManager.GetEffect<PartyFrame>().SetToken(this);
        }
        
        private InteractionTooltipData OnHoverCreature(IUncontrollableToken creature)
        {
            if (!creature.CanBeTargeted) return new InteractionTooltipData();

            if (!CanAttack)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "Unable to attack");
            }
            
            if (!CanAct)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "Unable to act");
            }
            
            return creature.IsInAttackRange(this)
                ? new InteractionTooltipData(InteractionMode.Attack, 
                    InteractionState.Allow, "Attack")
                : new InteractionTooltipData(InteractionMode.Attack, 
                    InteractionState.Abandon, "Attack", "Target\nnot in range");
        }

        private InteractionTooltipData OnHoverCard(Card card)
        {
            if (card.IsPlayingAnimation) return new InteractionTooltipData();

            string title = "";
            string subTitle = "This card\nis too far";
            InteractionMode mode = InteractionMode.None;
            InteractionState state = InteractionState.None;
            bool inWalkRange = PatternSearch.CheckPlus(Card.GridPosition, card.GridPosition, 1, false);


            if (inWalkRange)
            {
                mode = card.IsOpened 
                    ? InteractionMode.Move 
                    : InteractionMode.OpenCard;
                title = card.IsOpened 
                    ? "Move" : "Open";
                if (mode is InteractionMode.OpenCard && !CanInteractWithCards)
                {
                    state = InteractionState.Abandon;
                    subTitle = "Unable to interact\nwith cards";
                } else if(mode is InteractionMode.Move)
                {
                    bool canWalk = CanWalkOnCard(card);
                    state = canWalk ? InteractionState.Allow : InteractionState.Abandon;
                    subTitle = canWalk ? string.Empty : "Not enough MOV";
                };
            }
            else if(Card == card)
            {
                if (card.HasAvailableAction)
                {
                    subTitle = CanInteractWithCards ? "Unable to act" : "Unable to interact\nwith cards";
                    title = card.Scriptable.CardAction.Name;
                    mode = InteractionMode.Action;
                    state = CanInteractWithCards ? 
                        CanAct 
                            ? InteractionState.Allow 
                            : InteractionState.Abandon 
                        : InteractionState.Abandon;
                } 
            }
            else
            {
                mode = card.HasAvailableAction ? InteractionMode.Action : InteractionMode.OpenCard;
                state = InteractionState.Abandon;
            }
            
            return new InteractionTooltipData(mode: mode, state: state, actionTitle: title,  actionSubtitle: subTitle);
        }

        private void OnDragOnCreature(IUncontrollableToken creature)
        {
            if (!CanAttack || 
                !creature.CanBeTargeted || 
                !creature.IsInAttackRange(this))
            {
                AttackAnimatorManager.StopAnimation(transform, AttackType);
                return;
            }
            
            SetActionPoints(ActionPoints - 1);
            Attack(creature).Forget();
        }

        private void OnDragOnCard(Card card)
        {
            if (card.IsPlayingAnimation) return;
            if (Card == card)
            {
                UseCardAction(card);
                return;
            }
            
            if(!PatternSearch.CheckNeighbours(Card.GridPosition, card.GridPosition)) 
                return;

            bool canWalk = CanWalkOnCard(card); 
            Debug.Log($"OnDragOnCard, can walk: {canWalk}");
            if (canWalk)
                WalkOrFlee(card).Forget();
            else if(!card.IsOpened)
                OpenCard(card);
        }

        private async UniTask WalkOrFlee(Card card)
        {
            int creaturesAmount = Card.CreaturesAmount;
            if (creaturesAmount == 0 || Speed >= creaturesAmount)
            {
                await Walk(card);
                return;
            } 
            
            int roll = Random.Range(0, 6);
            await DiceManager.ThrowReplay(DiceManager.EventDiceSet, 1, new[] { roll });
            // TODO: ADD SPEED BONUS TEXT ANIMATION
            roll += Speed;
            if (roll <= creaturesAmount)
            {
                SetMovementPoints(MovementPoints - 1);
                return;
            }

            await Walk(card);
        }
        
        private void OpenCard(Card card)
        {
            if (!CanInteractWithCards || (!CanWalk && !ConsumeActionPointForMovement())) return;
            
            SetMovementPoints(MovementPoints - 1);
            card.Open();
        }

        private void UseCardAction(Card card)
        {
            if(!CanInteractWithCards || 
               !CanAct ||
               !card.HasAvailableAction) return;
            
            card.Scriptable.CardAction.Execute(card, this);
            SetActionPoints(ActionPoints - 1);
        }
        
        

        // IInteractableOnDrag
        public virtual void OnDragStart(InteractionResult result)
        {
            InteractionLine.Enable(result.IntersectionPoint);
            TokenBrowser.SelectToken(this);
            
            // UpdateOutlinesOnDragStart();
        }

        public InteractionTooltipData OnDrag(InteractionResult result)
        {
            bool valid = result.IsValid;
            InteractionLine.Enable(result.IntersectionPoint);
            if(!valid)
            {
                AttackAnimatorManager.StopAnimation(transform, AttackType);
                return null;
            }

            InteractionTooltipData tooltipData;
            switch (result.Target)
            {
                case IUncontrollableToken creature:
                    AttackAnimatorManager.StartAnimation(transform, AttackType, AttackVariation, result.IntersectionPoint);
                    tooltipData = OnHoverCreature(creature);
                    break;
                case Card card:
                    AttackAnimatorManager.StopAnimation(transform, AttackType);
                    tooltipData = OnHoverCard(card);
                    break;
                default:
                    AttackAnimatorManager.StopAnimation(transform, AttackType);
                    tooltipData = new InteractionTooltipData();
                    break;
            }
            
            InteractionLine.UpdatePosition(result.IntersectionPoint);
            InteractionLine.SetInteractableColor(tooltipData.State);
            return tooltipData;
        }

        public virtual void OnDragEnd(InteractionResult result)
        {
            // UpdateOutlinesOnDragEnd();
            
            InteractionLine.Disable();
            if (!result.IsValid)
            {
                return;
            }
            
            switch (result.Target)
            {
                case IUncontrollableToken creature:
                    OnDragOnCreature(creature);
                    break;
                case Card card:
                    OnDragOnCard(card);
                    break;
            }
        }

        public void GetInteractionTargets(List<IInteractable> targets)
        {
            if (CanWalk || CanAct)
            {
                PatternSearch.IterateNeighbours(Card.GridPosition, card =>
                {
                    if(card.IsOpened)
                    {
                        if (CanWalkOnCard(card)) targets.Add(card);
                    }
                    else if(CanInteractWithCards) targets.Add(card);
                });
            }

            if (CanAttack)
            {
                var inRange = GetCardsInAttackRange();
                foreach (Card card in inRange)
                {
                    targets.AddRange(card.Creatures);
                }
            }

            if (CanAct && Card.HasAvailableAction && CanInteractWithCards) targets.Add(Card);
        }
    }
}