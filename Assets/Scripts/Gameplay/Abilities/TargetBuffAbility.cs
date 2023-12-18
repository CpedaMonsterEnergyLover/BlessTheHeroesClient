using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class TargetBuffAbility : ActiveAbility, IEffectApplier
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int effectDuration;
        [SerializeField] private bool friendly;

        protected virtual ParticleSystem CastParticles => null;
        
        public override void OnCastStart()
        {
            if(CastParticles is not null) CastParticles.Play();
        }

        public override void OnCastEnd()
        {
            if(CastParticles is not null) CastParticles.Stop();
        }

        public override UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return default;
            token.BuffManager.ApplyEffect(this, effectToApply, effectDuration);
            return default;
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return friendly
                ? ValidateAlly(target)
                : ValidateEnemy(target);
        }
    }
}