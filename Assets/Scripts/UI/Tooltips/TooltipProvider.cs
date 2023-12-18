using Gameplay.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public abstract class TooltipProvider<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected abstract void ShowTooltip();
        protected abstract void HideTooltip();
        
        public T LastValue { get; protected set; }
        private bool shown;
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            ShowTooltip();
            shown = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!shown) return;
            HideTooltip();
            shown = false;
        }
    }
}