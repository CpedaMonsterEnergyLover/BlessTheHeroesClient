using System.Text;
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

        public int Mana => mana;
        public int Health => health;
        public int SpellPower => spellPower;
        public int AttackPower => attackPower;
        public int Defense => defense;

        
        
        public abstract bool CanEquipInSlot(int slot);

        public override int StackSize => 1;

        public virtual StringBuilder GetStatsStringBuilder()
        {
            StringBuilder sb = new StringBuilder();
            if(health > 0) sb.Append($"+{health} health\n");
            if(mana > 0) sb.Append($"+{mana} mana\n");
            if(spellPower > 0) sb.Append($"+{spellPower} spell power\n");
            if(attackPower > 0) sb.Append($"+{attackPower} attack power\n");
            if(defense > 0) sb.Append($"+{defense} defense");
            if (sb.Length > 0) sb.Insert(0, "When equipped, gives:\n");
            return sb;
        }
    }
}