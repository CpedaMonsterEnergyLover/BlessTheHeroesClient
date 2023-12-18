using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class Consumable : Item
    {
        public override bool AllowClick => TokenBrowser.SelectedToken is HeroToken {ActionPoints: > 0};
        public override int StackSize => 10;
        public override string CategoryName => "Consumable";

        
        
        public override void OnClickFromInventorySlot()
        {
            Debug.Log($"Consumed {name}");
        }
    }
}