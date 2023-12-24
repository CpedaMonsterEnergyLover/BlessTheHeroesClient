using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.Tokens;
using Scriptable;
using UnityEngine;

namespace Gameplay.BuffEffects
{
    public class RestorationBuffEffect : BuffEffect
    {
        [SerializeField] private int manaPerTick;
        [SerializeField] private int healthPerTick;
        [SerializeField] private DamageType healType;
        
        protected override void OnApplied()
        {
        }

        protected override void OnRemoved()
        {
        }

        protected override void OnTick()
        {
            IToken token = Manager.Token;
            
            if (healthPerTick > 0) token.Heal(healType, healthPerTick, Applier.Token, false);
            if (manaPerTick > 0) token.ReplenishMana(manaPerTick);
        }
    }
}