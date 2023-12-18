using Gameplay.Cards;
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
            executor.InventoryManager.AddItem(itemToGive, amount, out int left);
            if(left > 0) card.AddItemDrop(itemToGive);
        }
    }
}