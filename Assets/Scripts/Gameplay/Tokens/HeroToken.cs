using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.Inventory;
using Scriptable;
using Scriptable.AttackVariations;
using UI;
using UnityEngine;

namespace Gameplay.Tokens
{
    public class HeroToken : ControllableToken<Hero>
    {
        protected override int DefaultActionPoints => 2;
        protected override bool CanInteractWithCards => true;
        public override bool CanClick => true;
        public override int AttackDiceAmount => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackDiceAmount : 0;
        public override int DefenseDiceAmount => HasEquipmentInSlot(1) ? ((Armor) equipment[1]).DefenceDiceAmount : 0;
        public override BaseAttackVariation AttackVariation => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackVariation : null;
        public override DiceSet AttackDiceSet => DiceManager.AttackDiceSet;
        public override DiceSet MagicDiceSet => DiceManager.MagicDiceSet;
        public override DiceSet DefenseDiceSet => DiceManager.DefenceDiseSet;

        private readonly Equipment[] equipment = new Equipment[4];
        private readonly Ability[] equipmentAbilities = new Ability[4];

        public Ability[] EquipmentAbilities => equipmentAbilities.ToArray();
        
        
        
        // Class methods
        protected override void Init()
        {
            base.Init();
            for (var i = 0; i < 4; i++) 
                if(Scriptable.Equipment[i] is not null)
                    Equip(Scriptable.Equipment[i], i);
            
            SetHealth(MaxHealth);
            SetMana(MaxMana);
        }
        
        protected override void Die()
        {
            Debug.Log("Hero is dead");
        }

        public void Equip(Equipment item, int slot)
        {
            if(!item.CanEquipInSlot(slot)) return;
            
            Unequip(slot);
            InstantiateAbility(item, slot);
            maxManaBonus += item.Mana;
            SetMana(CurrentMana);
            maxHealthBonus += item.Health;
            SetHealth(CurrentHealth);
            attackPower += item.AttackPower;
            defense += item.Defense;
            spellPower += item.SpellPower;
            speedBonus += item.Speed;
            equipment[slot] = item;
            InvokeDataChangedEvent();
            TokenBrowser.Instance.UpdateEquipment(this);
        }

        private void InstantiateAbility(Equipment item, int slot)
        {
            if(!item.HasAbility) return;

            Ability inst = Instantiate(item.Ability, transform);
            inst.gameObject.name = item.Ability.Title;
            equipmentAbilities[slot] = inst;
            inst.SetToken(this);
        }

        private void RemoveAbility(Equipment item, int slot)
        {
            if(!item.HasAbility) return;
            Destroy(equipmentAbilities[slot]);
            equipmentAbilities[slot] = null;
        }

        public void Unequip(int slot)
        {
            var item = equipment[slot];
            if (item is null) return;
            
            RemoveAbility(item, slot);
            InventoryManager.Instance.AddItem(equipment[slot], transform.position, 1).Forget();
            maxManaBonus -= item.Mana;
            SetMana(CurrentMana - item.Mana < 0 ? 1 : CurrentMana - item.Mana);
            maxHealthBonus -= item.Health;
            SetHealth(CurrentHealth - item.Health < 0 ? 1 : CurrentHealth - item.Health);
            attackPower -= item.AttackPower;
            defense -= item.Defense;
            spellPower -= item.SpellPower;
            speedBonus -= item.Speed;
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