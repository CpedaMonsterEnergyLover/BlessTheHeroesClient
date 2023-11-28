using DG.Tweening;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class FireAbility : ActiveAbility
    {
        [Header("FireAbility Fields")]
        [SerializeField] protected ParticleSystem castBallParticles;
        [SerializeField] protected Light castBalllight;
        [SerializeField] protected float castBallDefaultIntensity;
        
        protected Tween lightTween;
        
        
        
        public override void OnCastStart() => AnimateCast(true);

        public override void OnCastEnd() => AnimateCast(false);

        protected void AnimateCast(bool state)
        {
            if(state) castBallParticles.Play();
            else castBallParticles.Stop();
            AnimateLight(state);
        }
        
        private void AnimateLight(bool state)
        {
            ManageTween();
            castBalllight.range = 1;
            lightTween = state 
                ? castBalllight.DOIntensity(castBallDefaultIntensity, 0.5f).OnComplete(() => lightTween = null) 
                : castBalllight.DOIntensity(0, 0.5f).OnComplete(() => lightTween = null);
        }

        protected void ManageTween()
        {
            if (lightTween is not null)
            {
                lightTween.Kill();
                lightTween = null;
            }
        }
    }
}