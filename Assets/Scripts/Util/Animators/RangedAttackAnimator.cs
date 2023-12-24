using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pooling;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Util.Animators
{
    public class RangedAttackAnimator : BaseAttackAnimator<RangedAttackVariation>
    {
        [SerializeField] private SpriteRenderer bowSpriteRenderer;
        [SerializeField] private Transform arrowSpriteTransform;
        [SerializeField] private Transform bowSpriteTransform;
        
        private const float BOW_OFFSET_AMOUNT = 0.1f;
        private const float STRETCH_ANIMATION_TIME = 0.25f;
        private const float ARROW_FROM = 0.65f;
        private const float ARROW_TO = 0.3f;
        
        private MaterialPropertyBlock materialPropertyBlock;
        private static readonly int VertexOffset = Shader.PropertyToID("_VertexOffset");
        
        public Vector3 ArrowPosition => bowSpriteTransform.position;
        
        private float StretchAmount
        {
            get => materialPropertyBlock.GetFloat(VertexOffset);
            set
            {
                materialPropertyBlock.SetFloat(VertexOffset, value);
                bowSpriteRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
        
        

        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            transform.localRotation = Quaternion.identity;
            bowSpriteRenderer.GetPropertyBlock(materialPropertyBlock);
        }


        protected override void PrepareVariation(RangedAttackVariation variation)
        {
        }

        public override void HoldAnimation(Vector3 point)
        {
            UpdateRotation(point);
        }

        protected override void StartAnimation(Transform self)
        {
            SetArrowActive(true);
            gameObject.SetActive(true);
            AnimateBowStretch(0.77f);
        }
        
        public override async UniTask AnimateAttack(Transform self, Transform target)
        {
            SetArrowActive(false);
            var arrow = PoolManager.GetEffect<EffectArrow>();
            arrow.SetPosition(ArrowPosition);
            arrow.SetRotation(GetRotation(target.position));
            await arrow.Shoot(target);
        }

        public override void StopAnimation(Transform self)
        {
            AnimateBowStretch(0);
        }

        private void AnimateBowStretch(float amount)
        {
            if (animationTween is not null)
            {
                Tween prev = animationTween;
                animationTween = null;
                prev.Kill();
            }
            
            bool direction = amount > 0;
            Vector3 bowForward = (bowSpriteTransform.up + bowSpriteTransform.right).normalized;
            arrowSpriteTransform.localPosition = bowForward * (direction ? ARROW_FROM : ARROW_TO);
            animationTween = DOTween.Sequence()
                .Append(DOTween.To(() => StretchAmount, value => StretchAmount = value, amount, STRETCH_ANIMATION_TIME))
                .Insert(0, bowSpriteTransform.DOLocalMove(bowForward * BOW_OFFSET_AMOUNT * (direction ? 1 : 0),
                    STRETCH_ANIMATION_TIME))
                .Insert(0, arrowSpriteTransform.DOLocalMove(bowForward * (direction ? ARROW_TO : ARROW_FROM), STRETCH_ANIMATION_TIME))
                .OnComplete(() =>
                {
                    if(animationTween is null) return;
                    if(!direction) 
                        gameObject.SetActive(false);
                    animationTween = null;
                });
        }

        private void SetArrowActive(bool isActive) => arrowSpriteTransform.gameObject.SetActive(isActive);
        
        public Quaternion GetRotation(Vector3 towards)
        {
            towards = transform.position - towards;
            towards.y = 0;
            float angle = Mathf.Atan2(towards.z, towards.x) * -Mathf.Rad2Deg - 45f;
            return Quaternion.Euler(90, angle, 90);
        }
        
        private void UpdateRotation(Vector3 towards)
        {
            Quaternion rotation = GetRotation(towards);
            arrowSpriteTransform.localRotation = rotation;
            bowSpriteTransform.localRotation = rotation;
        }
    }
}