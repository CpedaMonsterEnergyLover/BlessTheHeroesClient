using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using Scriptable;
using TMPro;
using UnityEngine;
using Util;
using Util.Enums;

namespace Effects
{
    public class EffectText : PoolObject
    {
        [SerializeField] private TMP_Text text;

        private static readonly Vector3 Offset = new(0, 0.115f, 0);
        
        public override void OnPool()
        {
            gameObject.SetActive(false);
        }

        public override void OnTakenFromPool()
        {
            transform.localScale = Vector3.one;
            text.SetText(string.Empty);
            gameObject.SetActive(true);
        }
        
        public async UniTask PlayDamage(Transform victimTransform, int damage, DamageType damageType, DamageImpact impact, Transform damageSourceTransform = null)
        {
            transform.position = victimTransform.position + Offset;
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            await AnimateAsync(damage, token, -1, damageType, 
                damageSourceTransform is null ? transform.position : damageSourceTransform.position, impact);
        }

        public async UniTask PlayHealing(Transform victimTransform, int healing, DamageType damageType, DamageImpact impact, Transform damageSourceTransform = null)
        {
            transform.position = victimTransform.position;
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            await AnimateAsync(healing, token, 1, damageType, 
                damageSourceTransform is null ? transform.position : damageSourceTransform.position, impact);
        }

        private async UniTask AnimateAsync(int damage, CancellationToken token, int direction, DamageType damageType, Vector3 damageSourcePoint, DamageImpact impact)
        {
            Color color = damageType.SecondaryColor;
            text.color = color;
            string impactString = impact is DamageImpact.Normal
                ? string.Empty
                : $"\n({impact})";
            text.SetText($"<color={color.ToHex()}>{(direction > 0 ? "+" : "-")}{damage} {damageType.Title}{impactString}");
            gameObject.SetActive(true);
            Vector3 point = transform.position + (transform.position - damageSourcePoint).normalized * 0.75f;
            Sequence currentSequence = DOTween.Sequence()
                .Append(transform.DOJump(new Vector3(point.x, Offset.y, point.z), 0.25f, 1, 2f)
                    .SetEase(Ease.OutCubic)
                    .Insert(0, transform.DOScale(1.25f, 1.5f))
                    .SetEase(Ease.OutCubic)
                    .Insert(1.5f, text.DOColor(color.WithAlpha(0), 0.5f)));
            
            await currentSequence.AsyncWaitForKill().AsUniTask().AttachExternalCancellation(token);
            
            gameObject.SetActive(false);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: token);
            Pool();
        }
    }
}