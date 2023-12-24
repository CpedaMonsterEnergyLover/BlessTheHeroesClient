using Gameplay.Abilities;
using Gameplay.Aggro;
using Scriptable;
using UnityEngine;

namespace Gameplay.BuffEffects
{
    public abstract class DamageOverTimeBuffEffect : BuffEffect
    {
        [SerializeField] private int damagePerTick;
        [SerializeField] private DamageType damageType;


        protected override void OnApplied()
        {
        }

        protected override void OnRemoved()
        {
        }

        protected override void OnTick()
        {
            Manager.Token.Damage(damageType, damagePerTick, Applier.Token, false);
        }
    }
}