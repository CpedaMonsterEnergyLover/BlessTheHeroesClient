using Gameplay.Abilities;
using Gameplay.Aggro;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.BuffEffects
{
    public class RestorationBuffEffect : BuffEffect
    {
        [SerializeField] private int manaPerTick;
        [SerializeField] private int healthPerTick;
        
        protected override void OnApplied()
        {
        }

        protected override void OnRemoved()
        {
        }

        protected override void OnTick()
        {
            IToken token = Manager.Token;
            IAggroManager aggroManager = Applier is Ability ability ? ability.Caster.IAggroManager : null;
            
            if (healthPerTick > 0) token.Heal(healthPerTick, aggroManager: aggroManager);
            if (manaPerTick > 0) token.ReplenishMana(manaPerTick);
        }
    }
}