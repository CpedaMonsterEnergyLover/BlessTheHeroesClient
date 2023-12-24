using Gameplay.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltips
{
    public abstract class TooltipProvider<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected abstract void ShowTooltip();
        protected abstract void HideTooltip();

        protected T LastValue { get; set; }
        private bool shown;
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InteractionManager.AnyInteractionActive) return;

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