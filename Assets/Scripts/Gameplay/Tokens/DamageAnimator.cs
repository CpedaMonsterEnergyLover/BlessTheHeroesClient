using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Util.Interface;

namespace Gameplay.Tokens
{
    public class DamageAnimator : MonoBehaviour, IHasAnimation
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TMP_Text text;

        private bool isPlayingAnimation;
        private Sequence currentSequence;
        private CancellationTokenSource cts = new();
        
        
        // Class methods
        public async UniTask PlayAsync(int damage, int delayMS)
        {
            if(cts is not null) cts.Cancel();
            cts = new CancellationTokenSource();
            await AnimationTask(damage, delayMS, cts.Token);
        }

        private async UniTask AnimationTask(int damage, int delayMS, CancellationToken token)
        {
            if (currentSequence is not null)
            {
                currentSequence.Kill();
                currentSequence = null;
            }
            
            text.SetText($"-{damage}");
            isPlayingAnimation = true;
            Transform spriteTransform = sprite.transform;
            Transform textTransform = text.transform;
            spriteTransform.localScale = Vector3.zero;
            textTransform.localScale = Vector3.zero;
            sprite.color = Color.white;
            text.color = Color.white;
            gameObject.SetActive(true);

            // Wait until attack animation hits the token
            await UniTask.Delay(TimeSpan.FromMilliseconds(delayMS), cancellationToken: token);
            
            currentSequence = DOTween.Sequence()
                // Sprite
                .Append(spriteTransform.DOScale(Vector3.one * 1.25f, 0.2f))
                .Append(spriteTransform.DOScale(Vector3.one, 0.05f))
                .AppendInterval(0.5f)
                .Append(spriteTransform.DOScale(Vector3.one * 1.65f, 0.25f))
                .Insert(0.75f, sprite.DOColor(new Color(1, 1, 1, 0f), 0.25f))
                
                // Text
                .Insert(0, textTransform.DOScale(Vector3.one, 0.25f))
                .Insert(0.75f, textTransform.DOScale(Vector3.one * 1.5f, 0.25f))
                .Insert(0.75f, text.DOFade(0, 0.25f));
            
            await currentSequence.AsyncWaitForKill().AsUniTask().AttachExternalCancellation(token);
            
            currentSequence = null;
            gameObject.SetActive(false);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: token);
            cts = null;
            isPlayingAnimation = false;
        }
        
        
        // IHasAnimation
        public bool IsPlayingAnimation => isPlayingAnimation;
    }
}