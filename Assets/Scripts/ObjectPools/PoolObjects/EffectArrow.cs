using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GameCycle;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Effects
{
    public class EffectArrow : PoolObject
    {
        [SerializeField] private TrailRenderer trail;

        private int despawnCounter;

        private void OnPlayersTurnStarted()
        {
            despawnCounter++;
            if(despawnCounter == 2) Pool();
        }

        public override void OnPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(false);
        }

        public override void OnTakenFromPool()
        {
            despawnCounter = 0;
            TurnManager.OnPlayersTurnStarted += OnPlayersTurnStarted;
        }

        private void OnDestroy()
        {
            TurnManager.OnPlayersTurnStarted -= OnPlayersTurnStarted;
        }

        public async UniTask Shoot(
            Transform target,
            float speed = 20f,
            float strength = 20f, 
            int vibratio = 30, 
            float randomness = 15f)
        {
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            trail.enabled = true;
            gameObject.SetActive(true);
            Vector2 treshHold = Random.insideUnitCircle * 0.15f;
            Tween tween = transform.DOMove(target.position + new Vector3(treshHold.x, 0, treshHold.y), speed)
                .SetEase(Ease.Linear)
                .SetSpeedBased();
            await tween.AsyncWaitForKill().AsUniTask().AttachExternalCancellation(token);
            transform.SetParent(target);
            transform.Rotate(new Vector3(1, 0, 0), 15);
            tween = transform.DOShakeRotation(0.6f, strength, vibratio, randomness);
            await tween.AsyncWaitForKill().AsUniTask().AttachExternalCancellation(token);
            trail.enabled = false;
        }
    }
}