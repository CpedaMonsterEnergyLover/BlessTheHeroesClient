using Camera;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Item = Scriptable.Item;

namespace Effects
{
    public class EffectLoot : EffectObject
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;

        public RectTransform Parent { get; set; }
        
        

        public override void OnPool()
        {
            gameObject.SetActive(false);
        }

        public override void OnTakenFromPool()
        {
        }

        public async UniTask AnimateGather(
            Vector3 startWorldPos,
            Item item)
        {
            image.sprite = item.Sprite;
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    Parent, 
                    MainCamera.Camera.WorldToScreenPoint(startWorldPos),
                    MainCamera.Camera, out var pos);
            rectTransform.position = Parent.TransformPoint(pos);
            rectTransform.localScale = Vector3.zero;
            
            gameObject.SetActive(true);
            Tween tween = DOTween.Sequence()
                .Append(rectTransform.DOScale(Vector3.one * 1.2f, 0.2f))
                .Append(rectTransform.DOScale(Vector3.one, 0.1f))
                .Append(rectTransform
                    .DOAnchorPos(new Vector2(-1, 1), 0.7f)
                    .SetEase(Ease.InQuad))
                .Insert(0.5f, 
                    rectTransform.DOScale(Vector3.zero, 0.5f));
            
            await tween.AsyncWaitForKill();
            Pool();
        }
    }
}