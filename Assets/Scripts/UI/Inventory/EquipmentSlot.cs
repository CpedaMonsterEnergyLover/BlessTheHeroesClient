using Gameplay.Abilities;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class EquipmentSlot : AbilitySlot
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private int index;

        private Scriptable.Equipment Equipment { get; set; }

        

        public override void UpdateIcon() => icon.sprite = Equipment.Sprite;

        public void SetItem(Scriptable.Equipment equipment, Ability ability)
        {
            Equipment = equipment;
            if(equipment.HasAbility)
                SetAbility(ability);
            else
                ClearAbility(equipment.Sprite);
        }

        public void ClearItem()
        {
            Equipment = null;
            ClearAbility(defaultSprite);
        }


        protected override void UpdateTooltipOnPointerEnter()
        {
            InventoryManager.Instance.EquipmentTooltip.SetItem(Equipment, Ability);
        }

        protected override void UpdateTooltipOnPointerExit()
        {
            InventoryManager.Instance.EquipmentTooltip.SetItem(null, null);
        }

        protected override void UpdateTooltipOnPointerClick()
        {
            InventoryManager.Instance.EquipmentTooltip.SetItem(null, null);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            if (Equipment is not null && 
                eventData.clickCount == 2 && 
                TokenBrowser.Instance.SelectedToken is HeroToken heroToken)
            {
                heroToken.Unequip(index);
                UpdateTooltipOnPointerClick();
            }
        }
    }
}