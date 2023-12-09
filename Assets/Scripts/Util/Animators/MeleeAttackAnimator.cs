using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util.Animators
{
    public class MeleeAttackAnimator : BaseAttackAnimator<MeleeAttackVariation>
    {
        protected override void StartAnimation(Transform self)
        {
            gameObject.SetActive(true);
        }

        protected override void PrepareVariation(MeleeAttackVariation variation)
        {
        }

        public override void HoldAnimation(Vector3 point)
        {
        }

        public override async UniTask AnimateAttack(Transform self, Transform target)
        {
            Vector3 prevPos = self.localPosition;
            Vector3 direction = target.position - self.position + prevPos + new Vector3(0, 0.1f, 0);
            direction -= direction.normalized * 0.25f;
            
            animationTween = self.DOLocalJump(direction, 0.25f, 1, 0.2f);
            await animationTween.AsyncWaitForKill();
            animationTween = self.DOLocalJump(prevPos, 0.5f, 1, 0.8f);
            
            await animationTween.AsyncWaitForKill().AsUniTask();
            
            animationTween = null;
        }

        public override void StopAnimation(Transform self)
        {
            gameObject.SetActive(false);
        }
    }
}