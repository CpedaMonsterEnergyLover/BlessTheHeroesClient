using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Effects;
using UI.Tooltips;
using UnityEngine;

namespace Gameplay.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        
        [SerializeField] private UI.Inventory inventoryUI;
        [SerializeField] private int slotsAmount;
        [SerializeField] private List<Item> itemsOnStart = new();
        [Header("Tooltips")] 
        [SerializeField] private InventoryTooltip inventoryTooltip;
        [SerializeField] private EquipmentTooltip equipmentTooltip;
        [SerializeField] private AbilityTooltip abilityTooltip;
        [Header("Debug")]
        public Scriptable.Item debug_ItemToAdd;
        public int debug_ToAddAmount;
        
        private List<Item> items = new();
        public InventoryTooltip InventoryTooltip => Instance.inventoryTooltip;
        public EquipmentTooltip EquipmentTooltip => Instance.equipmentTooltip;
        public AbilityTooltip AbilityTooltip => Instance.abilityTooltip;
        public int Coins { get; private set; }

        
        
        private InventoryManager() => Instance = this; 
        
        private void Start()
        {
            inventoryUI.CreateInventorySlots(slotsAmount);
            items = itemsOnStart.ToList();
            inventoryUI.UpdateInventory(items);
        }

        public async UniTask AddItem(Scriptable.Item itemToAdd, Vector3 giveFrom, int amount)
        {
            if (!CanFit(itemToAdd, amount, out int canFit, out Item sameItem))
            {
                // TODO: Play item destroy animation
                Debug.Log("Inventory is full");
                return;
            }
            
            if (sameItem is null) items.Add(new Item(itemToAdd, canFit));
            else sameItem.Amount += canFit;

            await EffectsManager.GetEffect<EffectLoot>()
                .AnimateGather(giveFrom, itemToAdd);
            
            inventoryUI.UpdateInventory(items);
        }

        public void RemoveItem(Scriptable.Item itemToRemove, int amount)
        {
            var item = items.FirstOrDefault(i => i.Scriptable.Equals(itemToRemove));
            if(item is null) return;
            item.Amount -= amount;
            if (item.Amount <= 0) items.Remove(item);
            inventoryUI.UpdateInventory(items);
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
            int emptySlots = slotsAmount - fullSlots;
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
            inventoryUI.UpdateInventory(items);
        }
        
        public bool CanAfford(int amount) => amount <= Coins;
        
        public void AddCoins(int amount)
        {
            if(amount <= 0) return;
            Coins += amount;
            inventoryUI.UpdateCoinsText(Coins);
        }

        public void RemoveCoins(int amount)
        {
            if(amount <= 0) return;
            Coins = Mathf.Clamp(Coins - amount, 0, int.MaxValue);
            inventoryUI.UpdateCoinsText(Coins);
        }
    }
}