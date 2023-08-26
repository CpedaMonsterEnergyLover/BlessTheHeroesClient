using Gameplay.Tokens;
using UI;
using UnityEngine;
using Util.Interface;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class Consumable : Item, IInventoryItem
    {
        public override bool AllowClick => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0};
        public override void OnClick()
        {
            Debug.Log($"Consumed {name}");
        }
        
        // IInventoryItem
        public override int StackSize => 10;
        public override string CategoryName => "Consumable";
        
    }
}