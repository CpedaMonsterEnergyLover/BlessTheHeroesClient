using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Aggro;
using Gameplay.Cards;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using UI;
using UI.Interaction;
using UnityEngine;
using Util;
using Util.Enums;
using Util.Interaction;
using Util.Patterns;

namespace Gameplay.Tokens
{
    public abstract class ControllableToken<T> : Token<T, ControllableAggroManager>,
        IControllableToken, IInteractableOnDrag
        where T : Scriptable.Token
    {
        public ControllableAggroManager AggroManager => aggroManager;

        public override bool CanInteract => 
            TurnManager.CurrentStage is TurnStage.PlayersTurn &&
            !Dead &&
            IToken.DraggedToken is null &&
            !IsPlayingAnimation &&
            (ActionPoints > 0 || MovementPoints > 0);

        public override Vector4 OutlineColor => ActionPoints == 0
            ? GlobalDefinitions.TokenOutlineYellowColor
            : GlobalDefinitions.TokenOutlineGreenColor;
        
        protected abstract bool CanInteractWithCards { get; }
        public override bool CanBeTargeted => false;


        
        public override void UpdateOutlineByCanInteract() => interactableOutline.SetEnabled(Initialized && CanInteract);

        protected override void OnPlayersTurnStarted()
        {
            ActionPoints = DefaultActionPoints;
            MovementPoints = Speed;
            UpdateOutlineByCanInteract();
            InvokeDataChangedEvent();
        }

        protected override void OnMonstersTurnStarted()
        {
            InteractableOutline.SetEnabled(false);
        }
        
        private InteractionTooltipData OnHoverCreature(IUncontrollableToken creature)
        {
            if (!creature.CanBeTargeted) return new InteractionTooltipData();

            if (AttackDiceAmount == 0)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "No weapon\nequipped");
            }
            
            if (ActionPoints == 0)
            {
                return new InteractionTooltipData(InteractionMode.Attack, InteractionState.Abandon,
                    "Attack", "Not enough\nACT");
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
                mode = card.IsOpened ? InteractionMode.Move : InteractionMode.OpenCard;
                title = card.IsOpened ? "Move" : "Open";
                if (mode is InteractionMode.OpenCard && !CanInteractWithCards)
                {
                    state = InteractionState.Abandon;
                    subTitle = "Companions\ncannot open cards";
                } else state = InteractionState.Allow;
            }
            else if(Card == card)
            {
                if (card.HasAvailableAction)
                {
                    subTitle = CanInteractWithCards ? "Not enough\nACT" : "Companions\ncannot interact\nwith cards";
                    title = card.Scriptable.CardAction.Name;
                    mode = InteractionMode.Action;
                    state = CanInteractWithCards ? 
                        ActionPoints > 0 
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
            if (ActionPoints == 0 || 
                !creature.CanBeTargeted || 
                !creature.IsInAttackRange(this) ||
                AttackDiceAmount == 0)
            {
                AttackAnimatorManager.StopAnimation(transform, AttackType);
                return;
            }
            
            SetActionPoints(ActionPoints - 1);
            Attack(creature).Forget();
        }

        private void OnDragOnCard(Card card)
        {
            if(card.IsPlayingAnimation) return;
            if (Card == card)
            {
                UseCardAction(card);
                return;
            }
            
            if(!PatternSearch.CheckNeighbours(Card.GridPosition, card.GridPosition)) 
                return;
            
            if (card.IsOpened &&
                card.HasSpaceForToken(this) &&
                (MovementPoints > 0 || ConsumeActionPointForMovement()))
                Walk(card).Forget();
            else
                OpenCard(card);
        }
        
        private void OpenCard(Card card)
        {
            if (!CanInteractWithCards ||
                (MovementPoints <= 0 &&
                 !ConsumeActionPointForMovement())) return;
            
            SetMovementPoints(MovementPoints - 1);
            card.Open();
        }

        private void UseCardAction(Card card)
        {
            if(!CanInteractWithCards || 
               ActionPoints == 0 ||
               !card.HasAvailableAction) return;
            
            card.Scriptable.CardAction.Execute(card, this);
            SetActionPoints(ActionPoints - 1);
        }
        
        

        // IInteractableOnDrag
        public virtual void OnDragStart(InteractionResult result)
        {
            ((IToken) this).InvokeStartDraggingEvent();
            InteractionLine.Enable(result.IntersectionPoint);
            TokenBrowser.Instance.SelectToken(this);
            
            UpdateOutlinesOnDragStart();
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
            UpdateOutlinesOnDragEnd();
            
            InteractionLine.Disable();
            if (!result.IsValid)
            {
                ((IToken) this).InvokeEndDraggingEvent();
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
            
            ((IToken) this).InvokeEndDraggingEvent();
        }

        private void UpdateOutlinesOnDragStart()
        {
            PatternSearch.IterateNeighbours(Card.GridPosition, pos =>
            {
                if(FieldManager.GetCard(pos, out Card card))
                {
                    if(card.IsOpened)
                        card.InteractableOutline.SetEnabled(!card.IsPlayingHeroesAnimation);
                    else 
                        card.InteractableOutline.SetEnabled(CanInteractWithCards && !card.IsPlayingHeroesAnimation);
                }
            });
            if (ActionPoints != 0)
            {
                if (CanInteractWithCards && Card.HasAvailableAction)
                    Card.InteractableOutline.SetEnabled(true);
                
                if(AttackDiceAmount != 0)
                    foreach (Card card in GetCardsInAttackRange()) 
                        card.OutlineAttackableCreatures(true);
            }
        }

        private void UpdateOutlinesOnDragEnd()
        {
            PatternSearch.IteratePlus(Card.GridPosition, 1,pos =>
            {
                if(FieldManager.GetCard(pos, out Card card)) card.InteractableOutline.SetEnabled(false);
            });
            foreach (Card card in GetCardsInAttackRange()) 
                card.OutlineAttackableCreatures(false);
        }
    }
}