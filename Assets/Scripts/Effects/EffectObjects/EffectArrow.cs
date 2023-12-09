using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Aggro;
using Gameplay.Tokens;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Effects
{
    public class EffectArrow : EffectObject
    {
        [SerializeField] private TrailRenderer trail;
        
        private Tween tween;
        
        public override void OnPool()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy() => tween?.Kill();

        public override void OnTakenFromPool() { }
        
        public async UniTask Shoot(
            Transform target,
            float speed = 20f,
            float strength = 20f, 
            int vibratio = 30, 
            float randomness = 15f)
        {
            trail.enabled = true;
            gameObject.SetActive(true);
            Vector2 treshHold = Random.insideUnitCircle * 0.15f;
            tween = transform.DOMove(target.position + new Vector3(treshHold.x, 0, treshHold.y), speed)
                .SetEase(Ease.Linear)
                .SetSpeedBased();
            await tween.AsyncWaitForKill();
            transform.SetParent(target);
            transform.Rotate(new Vector3(1, 0, 0), 15);
            tween = transform.DOShakeRotation(0.6f, strength, vibratio, randomness);
            await tween.AsyncWaitForKill().AsUniTask();
            trail.enabled = false;
            tween = null;
        }
    }
}