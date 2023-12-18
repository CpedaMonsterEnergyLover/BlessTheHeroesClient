using Gameplay.Tokens;
using Scriptable;
using BuffEffect = Gameplay.BuffEffects.BuffEffect;

namespace Gameplay.Abilities
{
    public class VoodooDollBuffEffect : BuffEffect
    {
        private IToken target;
        
        private void OnDamaged(DamageType damageType, int damage) => target?.Damage(damageType, damage);
        private void OnHealed(int heal) => target?.Heal(heal);
        private void OnManaReplenished(int mana) => target?.ReplenishMana(mana);

        protected override void OnApplied()
        {
            if(Applier is not VoodooDoll ability)
            {
                Manager.RemoveExact(this);
                return;
            }
            IToken token = Manager.Token;
            target = ability.LastTarget;
            token.OnDamaged += OnDamaged;
            token.OnHealed += OnHealed;
            token.OnManaReplenished += OnManaReplenished;
            target.OnDestroyed += OnDestroyed;
            token.OnDestroyed += OnDestroyed;
        }

        protected override void OnRemoved()
        {
            
        }

        private void OnDestroyed(IToken t)
        {
            IToken token = Manager.Token;
            token.OnDamaged -= OnDamaged;
            token.OnHealed -= OnHealed;
            token.OnManaReplenished -= OnManaReplenished;
            target.OnDestroyed -= OnDestroyed;
            token.OnDestroyed -= OnDestroyed;
            Manager.RemoveExact(this);
        }

        protected override void OnTick()
        {
        }
    }
}