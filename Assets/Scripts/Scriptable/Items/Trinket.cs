using Gameplay.Inventory;
using Gameplay.Tokens;
using UI;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Trinket")]
    public class Trinket : Equipment
    {
        public override string CategoryName => "Accessory";
        public override bool CanEquipInSlot(int slot)
        {
            return slot is 2 or 3;
        }
        
        public override bool AllowClick 
            => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0};
        
        public override void OnClick()
        {
            HeroToken hero = (HeroToken) TokenBrowser.Instance.SelectedToken;
            InventoryManager.Instance.RemoveItem(this, 1);

            int slot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 3 : 2;
            
            Equipment unequipped = null;
            if(hero.HasEquipmentInSlot(slot))
            {
                unequipped = hero.GetEquipmentAt(slot);
                hero.Unequip(slot);
            } 
            hero.Equip(this, slot);
            if(unequipped) hero.ReturnLostHealthAndMana(unequipped, this);
        }
    }
}