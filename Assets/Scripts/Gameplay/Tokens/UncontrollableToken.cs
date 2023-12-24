using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.Cards;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using MyBox;
using UnityEngine;
using Util;
using Util.Enums;
using Util.Patterns;
using Random = UnityEngine.Random;

namespace Gameplay.Tokens
{
    public abstract class UncontrollableToken<T> : Token<T, UncontrollableAggroManager>, IUncontrollableToken
        where T : Scriptable.Token
    {
        public override bool CanInteract => false;
        public override bool CanClick => true;
        public UncontrollableAggroManager AggroManager => aggroManager;

        public override Vector4 OutlineColor => TurnManager.CurrentStage is TurnStage.PlayersTurn
            ? GlobalDefinitions.TokenOutlineGreenColor
            : GlobalDefinitions.TokenOutlineRedColor;
        protected abstract float SharedLootDropModifier { get; }



        protected override void Die(IToken attacker)
        { 
            if(Scriptable.DropTable is null) return;
            var drops = Scriptable.DropTable.DropLoot();
            drops.AddRange(FieldManager.InstantiatedFloor.DropSharedLoot(SharedLootDropModifier));
            if(drops.Count == 0) return;

            // Если смерть от рук героя, выдать предметы ему, а если не получилось ему, то на карту
            if (attacker is HeroToken hero)
            {
                foreach (Scriptable.Item drop in drops)
                {
                    hero.InventoryManager.AddItem(drop, 1, out int left);
                    if(left > 0) Card.AddItemDrop(drop);
                }
                hero.InventoryManager.AddCoins(Scriptable.DropTable.DropCoins());
            }
            // Если смерть не от рук героя, положить предметы на карту
            else
            {
                Card.AddCoinDrop(Scriptable.DropTable.DropCoins());
                Card.AddItemDrops(drops);
            }
        }

        private async UniTask<bool> TryMakeAttack(IControllableToken target)
        {
            if (!target.CanBeTargeted ||
                !CanAttack || 
                !target.IsInAttackRange(this)) return false;
            
            SetActionPoints(ActionPoints - 1);
            await Attack(target);
            await UniTask.WaitUntil(() => !IsPlayingAnimation);
            return true;
        }
        
        private async UniTask<bool> TryWalk(IControllableToken target)
        {
            if (target is not null)
                return await TryWalkInAttackRange(target);
            
            return await TryWalkInRandomDirection();
        }

        private async UniTask<bool> TryWalkInRandomDirection()
        {
            Debug.Log($"{Scriptable.Name}'s turn *: TryWalkInRandomDirection");
            List<Card> cards = new();
            PatternSearch.IterateNeighbours(Card.GridPosition, pos =>
            {
                if (FieldManager.GetCard(pos, out Card card) && CanWalkOnCard(card)) cards.Add(card);
            });
            if (cards.Count <= 0) return false;

            if (previousCard is not null && cards.Count > 1 && cards.Contains(previousCard)) cards.Remove(previousCard);
            await Walk(cards[Random.Range(0, cards.Count)]);
            await UniTask.Delay(TimeSpan.FromMilliseconds(200));
            await UniTask.WaitUntil(() => !IsPlayingAnimation);
            return true;
        }

        private async UniTask<bool> TryWalkInAttackRange(IControllableToken target)
        {
            Debug.Log($"{Scriptable.Name}'s turn *: TryWalkInAttackRange");

            if (AttackType is AttackType.Ranged)
            {
                return await TryWalkInRandomDirection();
            }

            if (AttackType is AttackType.Melee)
            {
                Card targetCard = target.Card;
                if (!CanWalkOnCard(targetCard)) return false;
                await Walk(targetCard);
                return true;
            }
            return false;
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

        private IControllableToken TryGetAttackTarget()
        {
            List<IControllableToken> targets = new();

            switch (AttackType)
            {
                case AttackType.Melee:
                    targets.AddRange(Card.Heroes);
                    break;
                case AttackType.Ranged:
                    PatternSearch.IterateNeighbours(Card.GridPosition, v =>
                    {
                        if(FieldManager.GetCard(v, out Card n))
                            targets.AddRange(n.Heroes);
                    });
                    break;
                case AttackType.Magic:
                    PatternSearch.IteratePlus(Card.GridPosition, 1, v =>
                    {
                        if(FieldManager.GetCard(v, out Card n))
                            targets.AddRange(n.Heroes);
                    });
                    break;
            }

            return targets.Count == 0 
                ? null 
                : targets.MinBy(t => t.CurrentHealth);
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
            bool lockPosition = false;

            while ((CanAct || CanWalk) && counter <= 10)
            {
                counter++;

                IControllableToken aggroTarget = null;

                if (CanAct)
                {
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: GetAggroTarget");
                    if (CanAttack && !AggroManager.GetAggroTarget(out aggroTarget))
                    {
                        Debug.Log($"{Scriptable.Name}'s turn {counter}: TryGetAttackTarget");
                        aggroTarget = TryGetAttackTarget();
                    }
                    
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryCastAbility");
                    if(CanCast && await TryCastAbility())
                    {
                        continue;
                    }
                    
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryMakeAttack");
                    if(CanAttack && aggroTarget is not null && await TryMakeAttack(aggroTarget))
                    {
                        if(!aggroTarget.Dead) 
                            lockPosition = true;
                        continue;
                    }
                }
                
                if (!lockPosition && CanWalk)
                {
                    Debug.Log($"{Scriptable.Name}'s turn {counter}: TryWalk");
                    if (await TryWalk(aggroTarget))
                    {
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