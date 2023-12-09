using Cysharp.Threading.Tasks;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util.Animators
{
    public interface IAttackAnimator
    {
        public void StartAnimation(Transform self, BaseAttackVariation variation);
        public void HoldAnimation(Vector3 point);

        public UniTask AnimateAttack(Transform self, Transform target);
        
        public void StopAnimation(Transform self);
    }
}