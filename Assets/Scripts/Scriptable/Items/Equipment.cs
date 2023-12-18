using System.Text;
using Gameplay.Abilities;
using Gameplay.Tokens;
using MyBox;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    public abstract class Equipment : Item
    {
        [Header("Equipment data")]
        [SerializeField] private int mana;
        [SerializeField] private int health;
        [SerializeField] private int spellPower;
        [SerializeField] private int attackPower;
        [SerializeField] private int defense;
        [SerializeField] private int speed;
        [SerializeField] private bool hasAbility;
        [SerializeField, ConditionalField(nameof(hasAbility))]
        private Ability ability;


        public bool HasAbility => hasAbility;
        public Ability Ability => ability;
        public int Mana => mana;
        public int Health => health;
        public int SpellPower => spellPower;
        public int AttackPower => attackPower;
        public int Defense => defense;
        public int Speed => speed;

        
        
        public abstract bool CanEquipInSlot(int slot);
        public abstract int Slot { get; }

        public override int StackSize => 1;

        public virtual StringBuilder GetStatsStringBuilder()
        {
            StringBuilder sb = new StringBuilder();
            if (health > 0) sb.Append($"+{health} HP\n");
            if (mana > 0) sb.Append($"+{mana} MP\n");
            if (spellPower > 0) sb.Append($"+{spellPower} SPELL\n");
            if (attackPower > 0) sb.Append($"+{attackPower} ATK\n");
            if (defense > 0) sb.Append($"+{defense} DEF\n");
            if (sb.Length > 0) sb.Insert(0, "When equipped, gives\n");
            return sb;
        }
        
        public override void OnClickFromInventorySlot()
        {
            if(TokenBrowser.SelectedToken is not HeroToken hero) return;
            hero.InventoryManager.RemoveItem(this, 1);
            
            Equipment unequipped = null;
            if(hero.HasEquipmentInSlot(Slot))
            {
                unequipped = hero.GetEquipmentAt(Slot);
                hero.Unequip(Slot);
            }
            hero.Equip(this, Slot);
            if(unequipped) hero.ReturnLostHealthAndMana(unequipped, this);
        }
    }
}