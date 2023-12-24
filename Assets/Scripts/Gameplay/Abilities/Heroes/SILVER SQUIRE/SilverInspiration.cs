using Gameplay.BuffEffects;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class SilverInspiration : PassiveAbility, IEffectApplier
    {
        [SerializeField] private int manaReplenishAmount;
        [SerializeField] private BuffEffect silverInspirationBuffEffect;
        [SerializeField] private int buffDuration;
        
        private void OnEnable()
        {
            IToken.OnTokenMissGlobal += OnTokenMissGlobal;
        }

        private void OnDisable()
        {
            IToken.OnTokenMissGlobal -= OnTokenMissGlobal;
        }

        private void OnTokenMissGlobal(IToken token)
        {
            if(token is not HeroToken || token.BuffManager.FindEffect(silverInspirationBuffEffect, out _)) return;
            
            token.ReplenishMana(manaReplenishAmount);
            token.SetActionPoints(token.ActionPoints + 1);
            token.BuffManager.ApplyEffect(this, silverInspirationBuffEffect, buffDuration);
        }

        public IToken Token => Caster;
    }
}