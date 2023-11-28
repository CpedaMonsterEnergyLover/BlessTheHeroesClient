using Gameplay.Tokens;
using UI;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Consumable")]
    public class Consumable : Item
    {
        public override bool AllowClick => TokenBrowser.Instance.SelectedToken is HeroToken {ActionPoints: > 0};
        public override int StackSize => 10;
        public override string CategoryName => "Consumable";

        
        
        public override void OnClick()
        {
            Debug.Log($"Consumed {name}");
        }
    }
}