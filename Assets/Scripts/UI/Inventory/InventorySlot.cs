using Gameplay.Interaction;
using Gameplay.Inventory;
using TMPro;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text amountText;
        
        public Scriptable.Item Item { get; private set; }
        public int Amount { get; private set; }

        public void SetItem(Item item, int amount)
        {
            if (item is null)
            {
                icon.enabled = false;
                amountText.enabled = false;
            }
            else
            {
                icon.sprite = item.Scriptable.Sprite;
                amountText.SetText(amount.ToString());
                icon.enabled = true;
                amountText.enabled = amount > 1;
            }

            Item = item?.Scriptable;
            Amount = amount;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            TooltipManager.InventoryTooltip.SetItem(Item, Amount);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            TooltipManager.InventoryTooltip.SetItem(null, 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            if(Item is null) return;
            if (eventData.clickCount == 2 && Item.AllowClick)
            {
                Item.OnClickFromInventorySlot();
                TooltipManager.InventoryTooltip.SetItem(Item, Amount);
            }
        }
    }
}