using UnityEngine;
using Util.Interface;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Items/Resource")]
    public class Resource : Item, IInventoryItem
    {
        public override bool AllowClick => false;

        public override void OnClick() { }

        // IInventoryItem
        public override string CategoryName => "Resource";
        public override int StackSize => 20;
    }
}