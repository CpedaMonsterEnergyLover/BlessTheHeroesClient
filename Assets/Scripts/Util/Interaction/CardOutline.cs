using UnityEngine;

namespace Util.Interaction
{
    public class CardOutline : InteractableOutline
    {
        protected override Vector3 OutlineWidth => GlobalDefinitions.CardOutlineWidth;
    }
}