using System.Text;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UI;
using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : Equipment
    {
        [Header("Weapon data")]
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField] private AttackType attackType;

        public override string CategoryName => $"{attackType} Weapon";
        public int AttackDiceAmount => attackDiceAmount;
        public AttackType AttackType => attackType;
        
        
        public override bool CanEquipInSlot(int slot) => slot == 0;
        
        public override StringBuilder GetStatsStringBuilder()
        {
            return new StringBuilder()
                .Append($"Allows you to throw {attackDiceAmount} dice{(attackDiceAmount > 1 ? "s" : string.Empty)} when performing a physical attack.\n")
                .Append(base.GetStatsStringBuilder());
        }
        
        public override bool AllowClick 
            => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0} hero &&
               hero.Scriptable.AttackType == attackType;
        
        public override void OnClick()
        {
            HeroToken hero = (HeroToken) TokenBrowser.Instance.SelectedToken;
            InventoryManager.Instance.RemoveItem(this, 1);
            
            Equipment unequipped = null;
            if(hero.HasEquipmentInSlot(0))
            {
                unequipped = hero.GetEquipmentAt(0);
                hero.Unequip(0);
            }
            hero.Equip(this, 0);
            if(unequipped) hero.ReturnLostHealthAndMana(unequipped, this);
        }
    }
}