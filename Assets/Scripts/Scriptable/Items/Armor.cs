using System.Text;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UI;
using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Armor")]
    public class Armor : Equipment
    {
        [Header("Armor data")]
        [SerializeField, Range(1, 3)] private int defenceDiceAmount;
        [SerializeField] private ArmorType armorType;

        public override string CategoryName => "Armor";
        public int DefenceDiceAmount => defenceDiceAmount;
        public ArmorType ArmorType => armorType;

        
        
        public override StringBuilder GetStatsStringBuilder()
        {
            return new StringBuilder()
                .Append($"Allows you to throw {defenceDiceAmount} dice{(defenceDiceAmount > 1 ? "s" : string.Empty)} when blocking a physical attack.\n")
                .Append(base.GetStatsStringBuilder());
        }
        
        public override bool CanEquipInSlot(int slot) => slot == 1;
        
        public override bool AllowClick 
            => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0} hero &&
               (int) hero.Scriptable.ArmorType >= (int) armorType;
        
        public override void OnClick()
        {
            HeroToken hero = (HeroToken) TokenBrowser.Instance.SelectedToken;
            InventoryManager.Instance.RemoveItem(this, 1);

            Equipment unequipped = null;
            if(hero.HasEquipmentInSlot(1))
            {
                unequipped = hero.GetEquipmentAt(1);
                hero.Unequip(1);
            }
            hero.Equip(this, 1);
            if(unequipped) hero.ReturnLostHealthAndMana(unequipped, this);
        }
    }
}