using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Util.Tokens
{
    public class DamageAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private TMP_Text text;
        
        private Sequence currentSequence;
        private CancellationTokenSource cts = new();
        private bool isPlayingAnimation;
        public bool IsPlayingAnimation => isPlayingAnimation;

        

        private void OnDestroy()
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        // Class methods
        public async UniTask PlayDamage(int damage, int delayMS, Sprite overrideDamageSprite)
        {
            if(cts is not null) cts.Cancel();
            cts = new CancellationTokenSource();
            overrideDamageSprite = damage == 0
                ? GlobalDefinitions.DefensedDamageAnimationSprite
                : overrideDamageSprite;
            await AnimationTask(damage, delayMS, cts.Token, -1, overrideDamageSprite);
        }

        public async UniTask PlayHealing(int healing, int delayMS)
        {
            if(cts is not null) cts.Cancel();
            cts = new CancellationTokenSource();
            await AnimationTask(healing, delayMS, cts.Token, 1);
        }

        private async UniTask AnimationTask(int damage, int delayMS, CancellationToken token, int direction, Sprite overrideDamageSprite = null)
        {
            if (currentSequence is not null)
            {
                currentSequence.Kill();
                currentSequence = null;
            }

            if (direction < 0)
            {
                text.SetText($"-{damage}");
                text.color = Color.yellow;
                sprite.sprite = overrideDamageSprite is not null 
                    ? overrideDamageSprite 
                    : GlobalDefinitions.DamageAnimationSprite;
            } 
            else if (direction > 0)
            {
                text.SetText($"+{damage}");
                text.color = Color.green;
                sprite.sprite = GlobalDefinitions.HealingAnimationSprite;
            }
            
            isPlayingAnimation = true;
            Transform spriteTransform = sprite.transform;
            Transform textTransform = text.transform;
            spriteTransform.localScale = Vector3.zero;
            textTransform.localScale = Vector3.zero;
            sprite.color = Color.white;
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
    }
}