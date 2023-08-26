using UnityEngine;

namespace Gameplay.Inventory
{
    [System.Serializable]
    public class Item
    {
        [field:SerializeField] public Scriptable.Item Scriptable { get; private set; }
        [field: SerializeField] public int Amount { get; set; }

        public int GetStacks(out int lastStackItemsAmount)
        {
            int maxStack = Scriptable.StackSize;
            int fullStacks = Amount / maxStack;
            lastStackItemsAmount = Amount % maxStack;
            return fullStacks;
        }
        
        public Item(Scriptable.Item scriptable, int amount)
        {
            Scriptable = scriptable;
            Amount = amount;
        }
    }
}