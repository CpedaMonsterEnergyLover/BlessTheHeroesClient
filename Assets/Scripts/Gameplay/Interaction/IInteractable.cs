using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public interface IInteractable
    {
        public bool Dead { get; }
        public bool CanInteract { get; }
        public Vector4 OutlineColor { get; }
        public InteractableOutline InteractableOutline { get; }
        public void EnableOutline() => InteractableOutline.SetEnabled(true);
        public void DisableOutline() => InteractableOutline.SetEnabled(false);
        public void UpdateOutline() => InteractableOutline.SetEnabled(CanInteract);
        
        public delegate void InteractableEvent (IInteractable interactable);
        public event InteractableEvent OnDestroyed;
        public event InteractableEvent OnInitialized;
    }
}