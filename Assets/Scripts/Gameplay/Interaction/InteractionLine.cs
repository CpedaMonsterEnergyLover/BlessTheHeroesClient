using Unity.VisualScripting;
using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public class InteractionLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segmentsAmount;
        [SerializeField] private float height;
        [SerializeField] private AnimationCurve heightCurve;
        
        private bool isActive;
        
        
        
        // Unity methods
        private void Awake()
        {
            lineRenderer.positionCount = segmentsAmount + 1;
            SetInteractableColor(InteractionState.None);
        }
        
        
        
        // Class methods
        public void SetEnabled(bool isEnabled, Vector3 endPoint)
        {
            if(isActive == isEnabled) return;
            if (isEnabled)
            {
                UpdatePosition(endPoint);
                lineRenderer.enabled = true;
                isActive = true;
            }
            else
            {
                lineRenderer.enabled = false;
                isActive = false;
            }
        }

        public void Enable(Vector3 endPoint) => SetEnabled(true, endPoint);
        public void Disable() => SetEnabled(false, Vector3.zero);
        
        public void SetInteractableColor(InteractionState state) 
            => SetColor(InteractionColor.Get(state));

        public void UpdatePosition(Vector3 endPoint)
        {
            Vector3 direction = (endPoint - transform.position) / segmentsAmount;
            for (int i = 0; i <= segmentsAmount; i++)
            {
                Vector3 step = direction * i;
                lineRenderer.SetPosition(i, new Vector3(
                    step.x, 
                    heightCurve.Evaluate(i / (float) segmentsAmount) * height,
                    step.z));
            }
        }

        private void SetColor(Color c)
        {
            lineRenderer.startColor = c;
            lineRenderer.endColor = c.WithAlpha(0);
        }
    }
}