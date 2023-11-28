using Gameplay.BuffEffects;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities
{
    public abstract class AttackBuffEffectPassiveAbility<T> : PassiveAbility, IEffectApplier
    where T : BuffEffect
    {
        [SerializeField] private T effectToApply;
        [SerializeField] private int duration;
        
        protected override void OnTokenSet(IToken token)
        {
            token.OnAttackPerformed += OnAttackPerformed;            
        }

        private void OnAttackPerformed(IToken executor, IToken target, AttackType attacktype, int damage, int defensed)
        {
            target.BuffManager.ApplyEffect(this, effectToApply, duration);
        }
    }
}