using System.Collections.Generic;
using System.Linq;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Cards
{
    public partial class Card
    {
        [SerializeField] private InventoryManager inventoryManager;



        private void TryGiveItems()
        {
            var candidates = heroes
                .Where(h => h is HeroToken {Dead: false})
                .Cast<HeroToken>()
                .OrderBy(_ => Random.value)
                .ToArray();
            if(candidates.Length == 0) return;
            
            foreach (HeroToken hero in candidates)
            {
                hero.InventoryManager.AddCoins(inventoryManager.Coins);
                inventoryManager.RemoveCoins(inventoryManager.Coins);
                if(inventoryManager.IsEmpty) return;
                foreach (Item item in inventoryManager.Items)
                {
                    int amount = item.Amount;
                    Scriptable.Item scriptable = item.Scriptable;
                    hero.InventoryManager.AddItem(scriptable, amount, out int left);
                    if(amount != left) inventoryManager.RemoveItem(scriptable, amount - left);
                }
            }
        }

        public void AddItemDrops(List<Scriptable.Item> items)
        {
            foreach (Scriptable.Item item in items) 
                inventoryManager.AddItem(item, 1, out _);
            TryGiveItems();
        }
        
        public void AddItemDrop(Scriptable.Item item) => inventoryManager.AddItem(item, 1, out _);
        public void AddCoinDrop(int amount) => inventoryManager.AddCoins(amount);
    }
}