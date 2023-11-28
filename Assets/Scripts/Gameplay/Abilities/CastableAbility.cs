using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.GameCycle;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using MyBox;
using UnityEngine;
using Util.Patterns;

namespace Gameplay.Abilities
{
    public abstract class CastableAbility : Ability
    {
        [Separator("Pattern Settings")]
        [SerializeField] private Pattern pattern;
        [SerializeField, ConditionalField(nameof(pattern), true, Pattern.Single, Pattern.Neighbours)] 
        private int radius;
        [SerializeField, ConditionalField(nameof(pattern), true, Pattern.Single, Pattern.Neighbours)] 
        private bool includeCenter;
        [Separator("Ability Settings")]
        [SerializeField] protected int manacost;
        [SerializeField] protected int healthcost;
        [SerializeField] protected int energycost;
        [SerializeField] private uint cooldown;


        public virtual bool RequiresAct => true;
        public virtual int Manacost => manacost;
        public virtual int Healthcost => healthcost;
        public virtual int Energycost => energycost;
        public uint BaseCooldown => cooldown;
        public uint CurrentCooldown { get; private set; }


        
        public abstract bool ValidateTarget(IInteractable target);
        public abstract UniTask Cast(IInteractable target);

        public void GetTargetsList(List<IInteractable> targets)
        {
            PatternSearch.IteratePattern(pattern, Caster.TokenCard.GridPosition, card =>
            {
                if(ValidateTarget(card)) targets.Add(card);
                targets.AddRange(card.Creatures.Where(ValidateTarget));
                targets.AddRange(card.Heroes.Where(ValidateTarget));
            }, radius: radius, includeCenter: includeCenter);
        }

        public virtual bool ApproveCast(IToken token)
            => CurrentCooldown == 0 &&
               Manacost <= token.CurrentMana && 
               (!RequiresAct || token.ActionPoints > 0);
        
        private void Awake()
        {
            if(BaseCooldown != 0) TurnManager.OnPlayersTurnStarted += ReduceCooldown;
        }
        
        public void SetOnCooldown()
        {
            if(BaseCooldown == 0) return;
            CurrentCooldown = BaseCooldown;
            UpdateSlotCooldown();
        }

        private void ReduceCooldown()
        {
            if(BaseCooldown == 0 || CurrentCooldown == 0) return;
            CurrentCooldown--;
            UpdateSlotCooldown();
        }

        private void UpdateSlotCooldown()
        {
            if(AbilitySlot is null) return;
            AbilitySlot.UpdateCooldownText(CurrentCooldown);
            AbilitySlot.UpdateInteractable(Caster);
        }

        protected bool ValidateEnemy(IInteractable target)
        {
            return Caster switch
            {
                IControllableToken => target is IUncontrollableToken {CanBeTargeted: true},
                IUncontrollableToken => target is IControllableToken,
                _ => false
            };
        }

        protected bool ValidateAlly(IInteractable target)
        {
            return Caster switch
            {
                IControllableToken => target is IControllableToken,
                IUncontrollableToken => target is IUncontrollableToken {CanBeTargeted: true},
                _ => false
            };
        }

        protected bool ValidateOpenedCard(IInteractable target) => target is Card { IsOpened: true };
        
        protected bool ValidateCreature(IInteractable target) => target is CreatureToken {CanBeTargeted: true};

        protected bool ValidateHero(IInteractable target) => target is IControllableToken;
    }
}