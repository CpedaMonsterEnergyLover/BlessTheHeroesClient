using UnityEngine;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public class GameTable : MonoBehaviour, IInteractableOnClick
    {
        private bool dead;
        public bool Dead => dead;
        public bool CanInteract => false;
        public bool CanClick => true;
        public Vector4 OutlineColor => Vector4.zero;
        public InteractableOutline InteractableOutline => null;
        public event IInteractable.InteractableEvent OnDestroyed;
        public event IInteractable.InteractableEvent OnInitialized;

        public delegate void TableDoubleClickEvent(Vector3 position);
        public static event TableDoubleClickEvent OnDoubleClick;
        
        

        private void OnDestroy() => dead = true;

        public void OnClick(InteractionResult result, int clickCount)
        {
            if (clickCount > 1) OnDoubleClick?.Invoke(result.IntersectionPoint);
        }
    }
}