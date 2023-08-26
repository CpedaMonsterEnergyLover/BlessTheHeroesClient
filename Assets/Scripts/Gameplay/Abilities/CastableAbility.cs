using Cysharp.Threading.Tasks;
using Gameplay.GameCycle;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class CastableAbility : Ability
    {
        [SerializeField] private int manacost;
        [SerializeField] private uint cooldown;
        
        public int Manacost => manacost;
        public uint BaseCooldown => cooldown;
        public uint CurrentCooldown { get; private set; }
        
        public abstract UniTask Cast(IInteractable target);
        public virtual bool ApproveCast(HeroToken hero)
            => hero.ActionPoints != 0 &&
               CurrentCooldown == 0 &&
               Manacost <= hero.CurrentMana;
        
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