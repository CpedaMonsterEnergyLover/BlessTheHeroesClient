using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util.Animators
{
    public class MagicAttackAnimator : BaseAttackAnimator<MagicAttackVariation>
    {
        [SerializeField] protected ParticleSystem castballParticles;
        [SerializeField] protected ParticleSystem impactParticles;
        [SerializeField] protected ParticleSystem trailParticles;
        [SerializeField] protected ParticleSystem sparksParticles;
        [SerializeField] protected new Light light;
        [SerializeField] protected float castballDefaultIntensity;

        private Tween lightTween;
        private CancellationTokenSource cancellationToken;


        
        protected override void StartAnimation(Transform self)
        {
            transform.localPosition = new Vector3(0, 0.35f, 0);
            gameObject.SetActive(true);
            AnimateCast(true);
        }

        public override void HoldAnimation(Vector3 point)
        {
        }

        public override async UniTask AnimateAttack(Transform self, Transform target)
        {
            // Animate castball
            trailParticles.Play();
            var tween = transform
                .DOMove(target.position + new Vector3(0, 0.11f, 0), 0.25f)
                // .SetSpeedBased(true)
                .SetEase(Ease.Flash);
            await tween.AsyncWaitForKill();
            AnimateCast(false);
            
            // Animate Impact
            if (CurrentVariation.HasImpact)
            {
                AnimateExplosionLight();
                impactParticles.Play();
                await UniTask.WaitUntil(() => !impactParticles.isPlaying);
            }
        }

        public override void StopAnimation(Transform self)
        {
            AnimateCast(false);
        }
        
        protected override void PrepareVariation(MagicAttackVariation variation)
        {
            light.color = variation.LightColor;
            
            {
                ParticleSystem.ColorOverLifetimeModule colorModule = trailParticles.colorOverLifetime;
                colorModule.color = variation.TrailGradient;
            }
            
            
            castballParticles.gameObject.SetActive(variation.HasCastball);
            if (variation.HasCastball)
            {
                ParticleSystem.ColorOverLifetimeModule colorModule = castballParticles.colorOverLifetime;
                colorModule.color = variation.CastballGradient;
            } 
            
            impactParticles.gameObject.SetActive(variation.HasImpact);
            if (variation.HasImpact)
            {
                ParticleSystem.ColorOverLifetimeModule colorModule = impactParticles.colorOverLifetime;
                colorModule.color = variation.ImpactGradient;
                
            } 
            
            sparksParticles.gameObject.SetActive(variation.HasSparks);
            if (variation.HasSparks)
            {
                ParticleSystem.ColorOverLifetimeModule colorModule = sparksParticles.colorOverLifetime;
                colorModule.color = variation.SparksGradient;
            } 
        }        
        
        
        
        // Class methods
        private void AnimateCast(bool state)
        {
            AnimateLight(state);
            
            if(state)
            {
                cancellationToken?.Cancel();
                trailParticles.Play();
            }
            else
            {
                trailParticles.Stop();
                StopAnimation(ManageCancellation()).Forget();
            }
        }

        private async UniTaskVoid StopAnimation(CancellationToken cts)
        {
            await UniTask.WhenAll(
                UniTask.WaitUntil(() => lightTween is null, cancellationToken: cts),
                UniTask.WaitUntil(() => !trailParticles.isPlaying, cancellationToken: cts));
            
            cancellationToken.Dispose();
            cancellationToken = null;
            gameObject.SetActive(false);
        }
        
        private void AnimateLight(bool state)
        {
            ManageTween();
            light.range = 1;
            lightTween = state 
                ? light.DOIntensity(castballDefaultIntensity, 0.5f).OnComplete(() => lightTween = null) 
                : light.DOIntensity(0, 0.5f).OnComplete(() => lightTween = null);
        }
        
        private void AnimateExplosionLight()
        {
            ManageTween();
            light.intensity = castballDefaultIntensity * 1.5f;
            light.range = 3;
            lightTween = light.DOIntensity(0, 1f).OnComplete(() => lightTween = null);
        }

        private void ManageTween()
        {
            if (lightTween is not null)
            {
                lightTween.Kill();
                lightTween = null;
            }
        }

        private CancellationToken ManageCancellation()
        {
            cancellationToken?.Cancel();
            cancellationToken = new CancellationTokenSource();
            return cancellationToken.Token;
        }
    }
}