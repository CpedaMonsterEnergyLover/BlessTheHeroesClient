using Gameplay.BuffEffects;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class SigilOfProtectionBuffEffect : BuffEffect
    {
        [SerializeField] private int absorbAmount;
        [SerializeField] private int manaReplenishAmount;
        
        private int currentHealth;
        
        protected override void OnApplied()
        {
            currentHealth = absorbAmount;
            Manager.Token.OnDamageAbsorbed += DamageAbsorbed;
        }

        private int DamageAbsorbed(int incoming)
        {
            int absorbed = incoming <= currentHealth
                ? incoming 
                : currentHealth;
            currentHealth -= incoming;
            if(currentHealth <= 0)
            {
                if (Applier is Ability applier)
                {
                    applier.Caster.ReplenishMana(manaReplenishAmount);
                    Manager.Token.ReplenishMana(manaReplenishAmount);
                }
                Manager.RemoveExact(this);
            }
            return absorbed;
        }

        protected override void OnRemoved()
        {
            Manager.Token.OnDamageAbsorbed -= DamageAbsorbed;
        }

        protected override void OnTick()
        {
        }
    }
}