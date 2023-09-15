using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Inventory;
using UnityEngine;
using Util.Patterns;
using Util.Tokens;
using Random = UnityEngine.Random;

namespace Gameplay.Tokens
{
    public abstract class UncontrollableToken<T> : Token<T>, IUncontrollableToken
        where T : Scriptable.Token
    {
        public override bool CanBeTargeted => !Dead && !IsPlayingAnimation;
        public override bool CanInteract => false;
        public override bool CanClick => true;


        
        protected override void Init()
        {
            base.Init();
            ActionPoints = 1;
        }
        
        protected override void OnPlayersTurnStarted()
        {
            ActionPoints = 1;
            MovementPoints = Scriptable.Speed;
            InvokeDataChangedEvent();
            UpdateOutlineByCanInteract();
        }
        
        protected override void OnDeath()
        {
            if(Scriptable.DropTable is null) return;
            var drop = Scriptable.DropTable.Drop;
            InventoryManager.Instance.AddCoins(Scriptable.DropTable.Coins);
            foreach (Scriptable.Item item in drop) 
                InventoryManager.Instance.AddItem(item, 1);
        }

        private async UniTask<bool> TryMakeAttack()
        {
            if (AttackDiceAmount == 0 || ActionPoints == 0 || Card.HeroesAmount == 0) return false;
            
            var heroes = Card.Heroes;
            SetActionPoints(ActionPoints - 1);
            await Attack(heroes[Random.Range(0, heroes.Count)]);
            await UniTask.WaitUntil(() => !IsPlayingAnimation);
            return true;
        }

        private async UniTask<bool> TryWalkInRandomDirection()
        {
            List<Card> cards = new();
            PatternSearch.IterateNeighbours(Card.GridPosition, pos =>
            {
                if (FieldManager.GetCard(pos, out Card card) &&
                    card.IsOpened &&
                    card.HasSpaceForToken(this)) cards.Add(card);
            });
            if (cards.Count <= 0) return false;
            
            await Walk(cards[Random.Range(0, cards.Count)]);
            await UniTask.Delay(TimeSpan.FromMilliseconds(200));
            await UniTask.WaitUntil(() => !IsPlayingAnimation);
            return true;
        }

        private async UniTask<bool> TryCastAbility()
        {
            if (!FindAbilityToCast(out AutoAbility ability, out IInteractable target)) return false;

            if (ability.Manacost > 0 && !((IHasMana) this).DrainMana(ability.Manacost)) return false;
            
            SetActionPoints(ActionPoints - 1);
            ability.SetOnCooldown();
            Debug.Log($"{Scriptable.Name} casted ability {ability.Title}");
            await ability.Cast(target);
            
            return true;
        }

        private bool FindAbilityToCast(out AutoAbility ability, out IInteractable target)
        {
            target = null;
            ability = null;
            var abilities = Abilities
                .Where(a => a is AutoAbility)
                .Cast<AutoAbility>()
                .OrderBy(a => a.Priority);
            
            foreach (AutoAbility autoAbility in abilities)
            {
                if (autoAbility.ApproveCast(this) &&
                    autoAbility.GetTarget(out target))
                {
                    ability = autoAbility;
                    break;
                }
            }
            return ability is not null;
        }
        
        public async UniTask MakeTurn()
        {
            int counter = 0;

            int actions = ActionPoints;
            int movements = MovementPoints;
            
            while ((actions > 0 || movements > 0) 
                   && counter <= 10)
            {
                counter++;
                if (actions > 0)
                {
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryCastAbility");
                    if(await TryCastAbility())
                    {
                        actions--;
                        continue;
                    }
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryMakeAttack");
                    if(await TryMakeAttack())
                    {
                        actions--;
                        continue;
                    }
                }

                if (Card.HeroesAmount > 0) movements = 0;
                if (movements > 0)
                {
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryWalkInRandomDirection");
                    if (!await TryWalkInRandomDirection()) break;
                    movements--;
                }
            }
            
            if (counter > 10)
            {
                Debug.LogError($"{Scriptable.Name}: AI turn took more than 10 iterations, aborting process...");
            }
            
            
        }
    }
}