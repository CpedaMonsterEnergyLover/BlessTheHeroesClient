using Cysharp.Threading.Tasks;
using Gameplay.Dice;
using Gameplay.Inventory;
using Scriptable;
using UI;
using UnityEngine;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public class HeroToken : ControllableToken<Hero>
    {
        protected override int DefaultActionPoints => 2;
        protected override bool CanInteractWithCards => true;
        public override bool CanClick => true;
        public override int AttackDiceAmount => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackDiceAmount : 0;
        public override int DefenseDiceAmount => HasEquipmentInSlot(1) ? ((Armor) equipment[1]).DefenceDiceAmount : 0;
        public override DiceSet AttackDiceSet => DiceManager.AttackDiceSet;
        public override DiceSet MagicDiceSet => DiceManager.MagicDiceSet;
        public override DiceSet DefenseDiceSet => DiceManager.DefenceDiseSet;

        private readonly Equipment[] equipment = new Equipment[4];



        // Class methods
        protected override void Init()
        {
            base.Init();
            for (var i = 0; i < 4; i++) 
                if(Scriptable.Equipment[i] is not null)
                    Equip(Scriptable.Equipment[i], i);
            // Resets HP and MP to max after their equipment changes
            SetHealth(MaxHealth);
            SetMana(MaxMana);
            TokenBrowser.Instance.SelectFirst(this);
        }
        
        protected override void Die()
        {
            Debug.Log("Hero is dead");
        }

        public void Equip(Equipment item, int slot)
        {
            if(!item.CanEquipInSlot(slot)) return;
            
            Unequip(slot);
            maxManaBonus += item.Mana;
            SetMana(CurrentMana);
            maxHealthBonus += item.Health;
            SetHealth(CurrentHealth);
            attackPower += item.AttackPower;
            defense += item.Defense;
            spellPower += item.SpellPower;
            equipment[slot] = item;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        public void Unequip(int slot)
        {
            var item = equipment[slot];
            if (item is null) return;

            InventoryManager.Instance.AddItem(equipment[slot], transform.position, 1).Forget();
            maxManaBonus -= item.Mana;
            SetMana(CurrentMana - item.Mana < 0 ? 1 : CurrentMana - item.Mana);
            maxHealthBonus -= item.Health;
            SetHealth(CurrentHealth - item.Health < 0 ? 1 : CurrentHealth - item.Health);
            attackPower -= item.AttackPower;
            defense -= item.Defense;
            spellPower -= item.SpellPower;
            equipment[slot] = null;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        public void ReturnLostHealthAndMana(Equipment unequipped, Equipment equipped)
        {
            SetHealth(CurrentHealth + Mathf.Min(unequipped.Health, equipped.Health));
            SetMana(CurrentMana + Mathf.Min(unequipped.Mana, equipped.Mana));
        }
        
        public bool HasEquipmentInSlot(int slot) => equipment[slot] is not null;
        public Equipment GetEquipmentAt(int index) => equipment[index];
    }
}