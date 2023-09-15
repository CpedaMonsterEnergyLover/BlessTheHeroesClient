using Cysharp.Threading.Tasks;
using Gameplay.GameCycle;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class CastableAbility : Ability
    {
        [SerializeField] protected int manacost;
        [SerializeField] private uint cooldown;

        public virtual bool RequiresAct => true;
        public virtual int Manacost => manacost;
        public uint BaseCooldown => cooldown;
        public uint CurrentCooldown { get; private set; }
        
        
        
        public abstract UniTask Cast(IInteractable target);
        
        public bool ApproveCast(IToken token)
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
        }
    }
}