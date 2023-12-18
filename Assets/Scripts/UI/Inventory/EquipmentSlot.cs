using Gameplay.Abilities;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UI.Browsers;
using UI.Tooltips;
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
            TooltipManager.EquipmentTooltip.SetItem(Equipment, Ability);
        }

        protected override void UpdateTooltipOnPointerExit()
        {
            TooltipManager.EquipmentTooltip.SetItem(null, null);
        }

        protected override void UpdateTooltipOnPointerClick()
        {
            TooltipManager.EquipmentTooltip.SetItem(null, null);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            if (Equipment is not null && 
                eventData.clickCount == 2 && 
                TokenBrowser.SelectedToken is HeroToken heroToken)
            {
                heroToken.Unequip(index);
                UpdateTooltipOnPointerClick();
            }
        }
    }
}