using System.Linq;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Scriptable;
using Scriptable.AttackVariations;
using UnityEngine;

namespace Gameplay.Tokens
{
    public class HeroToken : ControllableToken<Hero>, IHeroToken, IItemReceiver
    {
        [SerializeField] private InventoryManager inventoryManager;
        
        protected override int DefaultActionPoints => 2;
        protected override bool CanInteractWithCards => true;
        public override bool CanClick => true;
        public override int AttackDiceAmount => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackDiceAmount : 0;
        public override int DefenseDiceAmount => HasEquipmentInSlot(1) ? ((Armor) equipment[1]).DefenceDiceAmount : 0;
        public override BaseAttackVariation AttackVariation => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).AttackVariation : null;
        public override DiceSet AttackDiceSet => DiceManager.AttackDiceSet;
        public override DiceSet MagicDiceSet => DiceManager.MagicDiceSet;
        public override DiceSet DefenseDiceSet => DiceManager.DefenceDiseSet;
        public override DamageType DamageType => HasEquipmentInSlot(0) ? ((Weapon) equipment[0]).DamageType : null;

        private readonly Equipment[] equipment = new Equipment[4];
        private readonly Ability[] equipmentAbilities = new Ability[4];
        public Ability[] EquipmentAbilities => equipmentAbilities.ToArray();
        public InventoryManager InventoryManager => inventoryManager;
        
        
        
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
        
        protected override void Die(IToken attacker)
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
            OnEquipped?.Invoke(this, slot, item);
        }

        public void Unequip(int slot)
        {
            var item = equipment[slot];
            if (item is null) return;
            
            RemoveAbility(item, slot);
            InventoryManager.AddItem(equipment[slot] /*, transform.position*/, 1, out _);
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
            OnUnequipped?.Invoke(this, slot, item);
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

        public void ReturnLostHealthAndMana(Equipment unequipped, Equipment equipped)
        {
            SetHealth(CurrentHealth + Mathf.Min(unequipped.Health, equipped.Health));
            SetMana(CurrentMana + Mathf.Min(unequipped.Mana, equipped.Mana));
        }

        public void Resurrect()
        {
            dead = false;
            SetHealth(1);
            SetMana(1);
            SetActionPoints(1);
            SetMovementPoints(1);
            OnResurrect?.Invoke(this);
        }
        
        public bool HasEquipmentInSlot(int slot) => equipment[slot] is not null;
        public Equipment GetEquipmentAt(int index) => equipment[index];

        public event IToken.TokenEvent OnResurrect;
        public event IHeroToken.EquipmentEvent OnEquipped;
        public event IHeroToken.EquipmentEvent OnUnequipped;
    }
}