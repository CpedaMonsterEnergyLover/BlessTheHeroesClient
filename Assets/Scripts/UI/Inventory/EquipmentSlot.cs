using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private int index;

        public Scriptable.Equipment Equipment { get; private set; }
        
        public void SetItem(Scriptable.Equipment equipment)
        {
            icon.sprite = equipment is null ? defaultSprite : equipment.Sprite;
            Equipment = equipment;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            InventoryManager.Instance.EquipmentTooltip.SetItem(Equipment);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            InventoryManager.Instance.EquipmentTooltip.SetItem(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            if (Equipment is not null && 
                eventData.clickCount == 2 && 
                TokenBrowser.Instance.SelectedToken is HeroToken heroToken)
            {
                heroToken.Unequip(index);
                InventoryManager.Instance.EquipmentTooltip.SetItem(Equipment);
            }
        }
    }
}