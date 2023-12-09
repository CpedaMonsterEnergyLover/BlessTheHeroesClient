using Gameplay.BuffEffects;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities
{
    public class AttackBuffEffectPassiveAbility : PassiveAbility, IEffectApplier
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int duration;
        
        protected override void OnTokenSet(IToken token)
        {
            token.OnBeforeAttackPerformed += BeforeAttackPerformed;            
        }

        private void BeforeAttackPerformed(IToken executor, IToken target, AttackType attacktype, int damage, int defensed)
        {
            target.BuffManager.ApplyEffect(this, effectToApply, duration);
        }
    }
}