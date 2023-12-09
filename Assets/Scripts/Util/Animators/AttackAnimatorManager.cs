using Cysharp.Threading.Tasks;
using Scriptable.AttackVariations;
using UnityEngine;
using Util.Enums;

namespace Util.Animators
{
    public class AttackAnimatorManager : MonoBehaviour
    {
        [SerializeField] private MeleeAttackAnimator melee;
        [SerializeField] private RangedAttackAnimator ranged;
        [SerializeField] private MagicAttackAnimator magic;

        private IAttackAnimator[] attackTypeToAnimator;
        
        public MeleeAttackAnimator Melee => melee;
        public RangedAttackAnimator Ranged => ranged;
        public MagicAttackAnimator Magic => magic;
        private bool IsActive { get; set; }
        


        private void Awake() => attackTypeToAnimator = new IAttackAnimator[] { melee, ranged, magic };

        public void StartAnimation(Transform self, AttackType attackType, BaseAttackVariation variation, Vector3 point)
        {
            Debug.Log($"Start Animation, variation: {variation}");
            if(variation is null) return;
            
            var animator = attackTypeToAnimator[(int)attackType];
            animator.HoldAnimation(point);
            
            if(IsActive) return;
            IsActive = true;
            animator.StartAnimation(self, variation);
        }

        public async UniTask AnimateAttack(Transform self, AttackType attackType, Transform target)
        {
            await attackTypeToAnimator[(int)attackType].AnimateAttack(self, target);
        }

        public void StopAnimation(Transform self, AttackType attackType)
        {
            if(!IsActive) return;

            attackTypeToAnimator[(int)attackType].StopAnimation(self);
            IsActive = false;
        }
    }
}