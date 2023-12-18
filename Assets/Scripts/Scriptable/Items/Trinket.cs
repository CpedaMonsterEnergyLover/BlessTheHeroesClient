using Gameplay.Tokens;
using UI.Browsers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Trinket")]
    public class Trinket : Equipment
    {
        public override string CategoryName => "Accessory";
        public override bool CanEquipInSlot(int slot)
        {
            return slot is 2 or 3;
        }

        public override int Slot => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 3 : 2;

        public override bool AllowClick 
            => TokenBrowser.SelectedToken is HeroToken {ActionPoints: > 0};
    }
}