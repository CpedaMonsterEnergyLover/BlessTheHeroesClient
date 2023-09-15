using Gameplay.Tokens;
using Gameplay.Tokens.Buffs;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities
{
    public class VenomousBite : PassiveAbility
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int duration;
        
        protected override void OnTokenSet(IToken token)
        {
            token.OnAttackPerformed += OnAttackPerformed;            
        }

        private void OnAttackPerformed(IToken executor, IToken target, AttackType attacktype, int damage, int defensed)
        {
            target.BuffManager.ApplyEffect(effectToApply, duration);
        }
    }
}