using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Resource")]
    public class Resource : Item
    {
        public override bool AllowClick => false;
        public override string CategoryName => "Resource";
        public override int StackSize => 20;
        public override void OnClickFromInventorySlot() { }
    }
}