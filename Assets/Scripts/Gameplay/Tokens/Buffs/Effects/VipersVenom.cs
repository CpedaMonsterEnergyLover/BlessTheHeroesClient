using UnityEngine;
using Util;

namespace Gameplay.Tokens.Buffs.Effects
{
    public class VipersVenom : BuffEffect
    {
        [SerializeField] private int damagePerTick;


        protected override void OnApplied()
        {
        }

        protected override void OnRemoved()
        {
        }

        protected override void OnTick()
        {
            Manager.Token.Damage(damagePerTick, overrideDamageSprite: GlobalDefinitions.PoisonDamageAnimationSprite);
        }
    }
}