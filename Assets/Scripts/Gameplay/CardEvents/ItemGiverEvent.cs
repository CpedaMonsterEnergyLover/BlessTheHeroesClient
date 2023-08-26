using Gameplay.GameField;
using Gameplay.Inventory;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Events
{
    public class ItemGiverEvent : CardEvent
    {
        [SerializeField] private Scriptable.Item itemToGive;
        [SerializeField] private int amount;
        
        public override void Execute(Card card, HeroToken executor)
        {
            InventoryManager.Instance.AddItem(itemToGive, amount);
        }
    }
}