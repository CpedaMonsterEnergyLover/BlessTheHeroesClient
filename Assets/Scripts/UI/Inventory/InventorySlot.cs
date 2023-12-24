using Gameplay.Interaction;
using Gameplay.Inventory;
using TMPro;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text amountText;

        public int Amount { get; private set; }
        public Scriptable.Item Item { get; private set; }
        public InventoryManager InventoryManager { get; set; }
        
        public delegate void ItemDragEvent(InventorySlot slot);
        public static event ItemDragEvent OnItemDragStart;
        public static event ItemDragEvent OnItemDragEnd;
        public static event ItemDragEvent OnItemDrag;
        
        public delegate void UsableHoverEvent(Scriptable.Usable usable);
        public static event UsableHoverEvent OnUsableHoverEnter;
        public static event UsableHoverEvent OnUsableHoverExit;
        

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

        
        // IPointerHandler
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InteractionManager.AnyInteractionActive) return;

            TooltipManager.InventoryTooltip.SetItem(Item, Amount);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.InventoryTooltip.SetItem(null, 0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(InteractionManager.AnyInteractionActive) return;
            
            if (eventData.clickCount == 2 && Item.AllowClick)
            {
                Item.Consume();
                TooltipManager.InventoryTooltip.SetItem(Item, Amount);
            }
        }

        
        // IDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(Item is not null)
            {
                TooltipManager.InventoryTooltip.SetItem(null, 0);
                OnItemDragStart?.Invoke(this);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(Item is not null) OnItemDragEnd?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(Item is not null) OnItemDrag?.Invoke(this);
        }
    }
}