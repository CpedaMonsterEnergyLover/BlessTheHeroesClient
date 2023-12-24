using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyBox;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Util.Enums;

namespace Pooling
{
    public class EffectText : Poolable
    {
        [FormerlySerializedAs("text")] 
        [SerializeField] private TMP_Text effectText;

        private static readonly Vector3 Offset = new(0, 0.115f, 0);
        
        public override void OnPool()
        {
            gameObject.SetActive(false);
        }

        public override void OnTakenFromPool()
        {
            transform.localScale = Vector3.one;
            effectText.SetText(string.Empty);
            gameObject.SetActive(true);
        }

        public async UniTask PlayText(Transform target, string text)
        {
            transform.position = target.position + Offset;
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            Color color = GlobalDefinitions.PhysicalDamageType.MainColor;
            await AnimateAsync(text, color, token, transform.position);
        }
        
        public async UniTask PlayDamage(Transform victimTransform, int damage, DamageType damageType, DamageImpact impact, Transform damageSourceTransform = null)
        {
            transform.position = victimTransform.position + Offset;
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            Color color = damageType.SecondaryColor;
            string impactString = impact is DamageImpact.Normal ? string.Empty : $"\n({impact})";
            string text = $"<size=2><color={color.ToHex()}>-{damage}</size>{damageType.Title}\n<size=1>{impactString}</size>";
            await AnimateAsync(text, color, token, damageSourceTransform is null ? transform.position : damageSourceTransform.position);
        }

        public async UniTask PlayHealing(Transform victimTransform, int healing, DamageType damageType, Transform damageSourceTransform = null)
        {
            transform.position = victimTransform.position + Offset;
            CancellationToken token = gameObject.GetCancellationTokenOnDestroy();
            Color color = damageType.SecondaryColor;
            string text = $"<color={color.ToHex()}>+{healing}";
            await AnimateAsync(text, color, token, damageSourceTransform is null ? transform.position : damageSourceTransform.position);
        }

        private async UniTask AnimateAsync(string text, Color color, CancellationToken token, Vector3 damageSourcePoint)
        {
            transform.localScale = Vector3.one;
            effectText.color = color;
            effectText.SetText(text);
            gameObject.SetActive(true);
            Vector3 point = transform.position + (transform.position - damageSourcePoint).normalized * 0.75f;
            Sequence currentSequence = DOTween.Sequence()
                .Append(transform.DOJump(new Vector3(point.x, Offset.y, point.z), 0.25f, 1, 2f)
                    .SetEase(Ease.OutCubic)
                    .Insert(0, transform.DOScale(1.25f, 1.5f))
                    .SetEase(Ease.OutCubic)
                    .Insert(1.5f, effectText.DOColor(color.WithAlpha(0), 0.5f)));
            
            await currentSequence.AsyncWaitForKill().AsUniTask().AttachExternalCancellation(token);
            
            gameObject.SetActive(false);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: token);
            Pool();
        }
    }
}