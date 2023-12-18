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


        protected abstract Sprite OverrideDamageSprite { get; }

        protected override void OnApplied()
        {
        }

        protected override void OnRemoved()
        {
        }

        protected override void OnTick()
        {
            IAggroManager aggroManager = Applier is Ability ability ? ability.Caster.IAggroManager : null;
            
            Manager.Token.Damage(damageType, damagePerTick, 
                aggroReceiver: aggroManager);
        }
    }
}