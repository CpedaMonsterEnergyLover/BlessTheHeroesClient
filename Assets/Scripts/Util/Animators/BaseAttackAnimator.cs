using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util.Animators
{
    public abstract class BaseAttackAnimator<T> : MonoBehaviour, IAttackAnimator
    where T : BaseAttackVariation
    {
        protected Tween animationTween;
        protected T CurrentVariation { get; private set; }

        
        
        public void StartAnimation(Transform self, BaseAttackVariation variation)
        {
            if(variation is not T casted)
                Debug.LogWarning($"Expected attack variation of type {typeof(T)}, got {variation.GetType()}. Animation cannot be started.");
            else
            {
                if (!variation.Equals(CurrentVariation))
                {
                    PrepareVariation(casted);
                    CurrentVariation = casted;
                }
                StartAnimation(self);
            }
        }

        protected abstract void PrepareVariation(T variation);
        
        public abstract void HoldAnimation(Vector3 point);

        public abstract UniTask AnimateAttack(Transform self, Transform target);

        public abstract void StopAnimation(Transform self);
        protected abstract void StartAnimation(Transform self);
    }
}