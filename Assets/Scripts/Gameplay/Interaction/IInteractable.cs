using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public interface IInteractable
    {
        public bool CanInteract { get; }
        public InteractableOutline Outline { get; }
        public Vector4 OutlineColor { get; }
        public void UpdateOutlineByCanInteract();
    }
}