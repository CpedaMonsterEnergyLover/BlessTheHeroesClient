using Camera;
using UnityEngine;

namespace UI.Elements
{
    public class MouseSticker : MonoBehaviour
    {
        [SerializeField] private Vector2 padding;
        
        private new UnityEngine.Camera camera;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            camera = MainCamera.Camera;
            rectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        private void OnEnable()
        {
            UpdatePosition();
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    rectTransform, 
                    Input.mousePosition,
                    camera, out var pos);
            transform.position = rectTransform.TransformPoint(pos + padding);
        }
    }
}