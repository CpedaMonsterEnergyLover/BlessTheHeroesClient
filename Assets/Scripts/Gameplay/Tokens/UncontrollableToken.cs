using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Inventory;
using UnityEngine;
using Util;
using Util.Patterns;
using Util.Tokens;
using Random = UnityEngine.Random;

namespace Gameplay.Tokens
{
    public abstract class UncontrollableToken<T> : Token<T, UncontrollableAggroManager>, IUncontrollableToken
        where T : Scriptable.Token
    {
        public override bool CanBeTargeted => !Dead && !IsPlayingAnimation;
        public override bool CanInteract => false;
        public override bool CanClick => true;
        public UncontrollableAggroManager AggroManager => aggroManager;

        public override Vector4 OutlineColor => TurnManager.CurrentStage is TurnStage.PlayersTurn
            ? GlobalDefinitions.TokenOutlineGreenColor
            : GlobalDefinitions.TokenOutlineRedColor;



        public override void UpdateOutlineByCanInteract() => interactableOutline.SetEnabled(false);
        
        protected override void OnPlayersTurnStarted()
        {
            ActionPoints = DefaultActionPoints;
            MovementPoints = Scriptable.Speed;
            InvokeDataChangedEvent();
            UpdateOutlineByCanInteract();
        }

        public override async UniTask Despawn()
        {
            Card card = TokenCard;
            await base.Despawn();
            card.TryClearAggro();
        }

        protected override void Die()
        { 
            if(Scriptable.DropTable is null) return;
            var drops = Scriptable.DropTable.Drop;
            InventoryManager.Instance.AddCoins(Scriptable.DropTable.Coins);
            if(drops.Count != 0) DropItemsOnDeath(transform.position, drops).Forget();
        }
 
        private async UniTask DropItemsOnDeath(Vector3 pos, List<Scriptable.Item> drops)
        {
            int delay = 1000 / drops.Count;

            foreach (Scriptable.Item drop in drops)
            {
                InventoryManager.Instance.AddItem(drop, pos, 1).Forget();
                await UniTask.Delay(delay);
            }
        }

        private async UniTask<bool> TryMakeAttack()
        {
            if (AttackDiceAmount == 0 || 
                ActionPoints == 0 ||  
                !TryGetAttackTarget(out IToken target)) return false;
            
            SetActionPoints(ActionPoints - 1);
            await Attack(target);
            await UniTask.WaitUntil(() => !IsPlayingAnimation);
            return true;
        }

        private bool TryGetAttackTarget(out IToken target)
        {
            target = null;
            var heroes = Card.Heroes.ToArray();
            int len = heroes.Length;
            if (len == 0) return false;
            if (len == 1)
            {
                target = heroes[0];
                return true;
            }

            float max = heroes.Max(h => h.AggroManager.AggroLevel);
            var same = heroes.Where(h => h.AggroManager.AggroLevel >= max).ToArray();
            target = same[Random.Range(0, same.Length)];
            return target is not null;
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

            if (ability.Manacost > 0 && !DrainMana(ability.Manacost)) return false;
            
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

            if (actions > 0 && movements > 0 && AggroManager.TryReaggro(out Card redirect)) 
                await Walk(redirect);

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

 
                if (Card.HeroesAmount == 0 && movements > 0)
                {
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryWalkInRandomDirection");
                    if (await TryWalkInRandomDirection())
                    {
                        movements--;
                        continue;
                    }
                }
                
                break;
            }
            
            if (counter > 10)
            {
                Debug.LogError($"{Scriptable.Name}: AI turn took more than 10 iterations, aborting process...");
            }
            
            
        }
    }
}