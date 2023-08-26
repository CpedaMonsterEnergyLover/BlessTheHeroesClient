using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Inventory;
using Scriptable;
using UI.Interaction;
using Util.Patterns;
using Random = UnityEngine.Random;

namespace Gameplay.Tokens
{
    public class CreatureToken : Token<Creature>
    {
        public override bool CanClick => true;
        public override bool CanInteract => 
            !Dead && 
            Controllable && 
            !IsPlayingAnimation && 
            (ActionPoints > 0 || MovementPoints > 0);
        public override bool CanBeTargeted => !Dead && !Controllable && !IsPlayingAnimation;
        public override int Speed => 1;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => Scriptable.DefenseDiceAmount;
        public override DiceSet AttackDiceSet => Scriptable.OverrideAttackDice(out DiceSet dice) ? dice : FieldManager.MonsterAttackDice;
        public override DiceSet MagicDiceSet => Scriptable.OverrideMagicDice(out DiceSet dice) ? dice : FieldManager.MonsterMagicDice;
        public override DiceSet DefenseDiceSet => Scriptable.OverrideDefenseDice(out DiceSet dice) ? dice : FieldManager.MonsterDefenseDice;



        // Class methods
        protected override void Init()
        {
            base.Init();
            Controllable = false;
            ActionPoints = 1;
            MovementPoints = 1;
        }

        protected override void OnDeath()
        {
            if(Scriptable.DropTable is null) return;
            var drop = Scriptable.DropTable.Drop;
            InventoryManager.Instance.AddCoins(Scriptable.DropTable.Coins);
            foreach (Scriptable.Item item in drop) 
                InventoryManager.Instance.AddItem(item, 1);
        }

        protected override InteractionTooltipData OnHoverCreature(CreatureToken creature)
        {
            return new InteractionTooltipData();
        }

        protected override InteractionTooltipData OnHoverCard(Card card)
        {
            return new InteractionTooltipData();
        }

        protected override InteractionTooltipData OnHoverOther()
        {
            return new InteractionTooltipData();
        }

        protected override void OnDragOnCreature(CreatureToken creature)
        {
        }

        protected override void OnDragOnCard(Card card)
        {
        }

        protected override void OnPlayersTurnStarted()
        {
            ActionPoints = 1;
            MovementPoints = 1;
            InvokeDataChangedEvent();
            UpdateOutlineByCanInteract();
        }

        protected override void OnMonstersTurnStarted()
        {
            
        }

        public async UniTask MakeTurn()
        {
            if(Controllable) return;

            bool attackMade = await TryMakeAttack();
            if (!attackMade && MovementPoints != 0)
            {
                List<Card> cards = new();
                PatternSearch.IterateNeighbours(Card.GridPosition, (pos) =>
                {
                    if(FieldManager.GetCard(pos, out Card card) && card.IsOpened && !card.FullOfCreatures)
                        cards.Add(card);
                });
                if(cards.Count > 0)
                {
                    Walk(cards[Random.Range(0, cards.Count)]);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(200));
                    await UniTask.WaitUntil(() => !IsPlayingAnimation);
                }
                await TryMakeAttack();
            }
            await UniTask.Delay(TimeSpan.FromMilliseconds(200));
        }

        private async UniTask<bool> TryMakeAttack()
        {
            if (AttackDiceAmount != 0 && ActionPoints != 0 && Card.HasHeroes)
            {
                var heroes = Card.Heroes;
                await Attack(heroes[Random.Range(0, heroes.Count)]);
                await UniTask.WaitUntil(() => !IsPlayingAnimation);
                return true;
            }

            return false;
        }
    }
}