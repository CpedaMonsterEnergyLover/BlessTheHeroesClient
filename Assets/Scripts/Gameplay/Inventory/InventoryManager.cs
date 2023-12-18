using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [FormerlySerializedAs("slotsAmount")] 
        [SerializeField] private int capacity;

        private /*readonly */List<Item> items = new();

        public int Coins { get; private set; }
        public int Capacity => capacity;
        public Item[] Items => items.ToArray();
        public bool IsEmpty => items.Count == 0;
        
#if UNITY_EDITOR
        [Header("Debug")]
        public Scriptable.Item debug_ItemToAdd;
        public int debug_ToAddAmount;
#endif

        public delegate void InventoryItemsUpdateEvent(Item[] items);
        public event InventoryItemsUpdateEvent OnItemsUpdate;
        public delegate void InventoryCoinsUpdateEvent(int coins);
        public event InventoryCoinsUpdateEvent OnCoinsUpdate;
        
        

        public void AddItem(Scriptable.Item itemToAdd,/* Vector3 giveFrom, */int amount, out int leftAmount)
        {
            leftAmount = amount;
            if (!CanFit(itemToAdd, amount, out int canFit, out Item sameItem)) return;

            if (sameItem is null) 
                items.Add(new Item(itemToAdd, canFit));
            else 
                sameItem.Amount += canFit;

            leftAmount = amount - canFit;
            /*await EffectsManager.GetEffect<EffectLoot>().AnimateGather(giveFrom, itemToAdd);*/
            
            OnItemsUpdate?.Invoke(Items);
        }

        public void RemoveItem(Scriptable.Item itemToRemove, int amount)
        {
            var item = items.FirstOrDefault(i => i.Scriptable.Equals(itemToRemove));
            if(item is null) return;
            item.Amount -= amount;
            if (item.Amount <= 0) items.Remove(item);
            
            OnItemsUpdate?.Invoke(Items);
        }

        private bool CanFit(Scriptable.Item itemToAdd, int amount, out int canFit, out Item sameItem)
        {
            canFit = 0;
            sameItem = null;
            int sameLastStack = 0;
            int fullSlots = 0;
            foreach (Item item in items)
            {
                int stacks = item.GetStacks(out int lastStack);
                fullSlots += stacks;
                if (lastStack != 0) fullSlots++;
                if (item.Scriptable.Equals(itemToAdd))
                {
                    sameItem = item;
                    sameLastStack = lastStack;
                }
            }

            int possibleMaximumAmount;
            int emptySlots = capacity - fullSlots;
            if (sameItem is null)
                possibleMaximumAmount = emptySlots * itemToAdd.StackSize;
            else
            {
                possibleMaximumAmount = emptySlots * itemToAdd.StackSize;
                if (sameLastStack != 0) possibleMaximumAmount += itemToAdd.StackSize - sameLastStack;
            }

            canFit = possibleMaximumAmount < amount ? possibleMaximumAmount : amount;
            return canFit != 0;
        }

        public void Clear()
        {
            items.Clear();
            OnItemsUpdate?.Invoke(Items);
        }
        
        public bool CanAfford(int amount) => amount <= Coins;
        
        public void AddCoins(int amount)
        {
            if(amount <= 0) return;
            Coins += amount;
            OnCoinsUpdate?.Invoke(Coins);
        }

        public void RemoveCoins(int amount)
        {
            if(amount <= 0) return;
            Coins = Mathf.Clamp(Coins - amount, 0, int.MaxValue);
            OnCoinsUpdate?.Invoke(Coins);
        }
    }
}