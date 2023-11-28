using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public class GameTable : MonoBehaviour, IInteractable
    {
        // IInteractable
        public bool CanInteract => false;
        public InteractableOutline Outline => null;
        public Vector4 OutlineColor => Vector4.zero;
        public void UpdateOutlineByCanInteract() { }
    }
}