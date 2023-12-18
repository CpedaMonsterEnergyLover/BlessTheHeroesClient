using Gameplay.Tokens;
using UnityEngine;
using Util;
using Util.Enums;

namespace Gameplay.Abilities.Items
{
    public class PoisonedBlade : PassiveSubscribeAbility
    {
        [SerializeField] private int damage;
        
        
        
        protected override void OnSubscribed(IToken token) => token.OnAfterAttackPerformed += OnAfterAttackPerformed;

        protected override void OnUnSubscribed(IToken token) => token.OnAfterAttackPerformed -= OnAfterAttackPerformed;

        private void OnAfterAttackPerformed(IToken executor, IToken target, AttackType attackType, int dmg,
            int defensed)
        {
            target.Damage(GlobalDefinitions.PoisonDamageType, damage, aggroReceiver: Caster.IAggroManager);
        }
    }
}