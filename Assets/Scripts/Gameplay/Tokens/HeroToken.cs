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
            ActionPoints = 2;
            for (var i = 0; i < 4; i++) 
                if(Scriptable.Equipment[i] is not null)
                    Equip(Scriptable.Equipment[i], i);
            // Resets HP and MP to max after their equipment changes
            ((IHasHealth) this).SetHealth(MaxHealth);
            ((IHasMana) this).SetMana(MaxMana);
            TokenBrowser.Instance.SelectFirst(this);
        }
        
        protected override void OnDeath()
        {
            Debug.Log("Hero is dead");
        }
        
        public void Equip(Equipment item, int slot)
        {
            if(!item.CanEquipInSlot(slot)) return;
            
            Unequip(slot);
            maxManaBonus += item.Mana;
            ((IHasMana) this).SetMana(CurrentMana);
            maxHealthBonus += item.Health;
            ((IHasHealth) this).SetHealth(CurrentHealth);
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

            InventoryManager.Instance.AddItem(equipment[slot], 1);
            maxManaBonus -= item.Mana;
            ((IHasMana) this).SetMana(CurrentMana - item.Mana < 0 ? 1 : CurrentMana - item.Mana);
            maxHealthBonus -= item.Health;
            ((IHasHealth) this).SetHealth(CurrentHealth - item.Health < 0 ? 1 : CurrentHealth - item.Health);
            attackPower -= item.AttackPower;
            defense -= item.Defense;
            spellPower -= item.SpellPower;
            equipment[slot] = null;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        public void ReturnLostHealthAndMana(Equipment unequipped, Equipment equipped)
        {
            ((IHasHealth) this).SetHealth(CurrentHealth + Mathf.Min(unequipped.Health, equipped.Health));
            ((IHasMana) this).SetMana(CurrentMana + Mathf.Min(unequipped.Mana, equipped.Mana));
        }
        
        public bool HasEquipmentInSlot(int slot) => equipment[slot] is not null;
        public Equipment GetEquipmentAt(int index) => equipment[index];
    }
}