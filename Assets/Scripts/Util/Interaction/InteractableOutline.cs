using Gameplay.Interaction;
using UnityEngine;

namespace Util.Interaction
{
    public abstract class InteractableOutline : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        private MaterialPropertyBlock materialPropertyBlock;
        private Vector4 currentColor = Vector4.zero;
        protected IInteractable interactable;
        protected abstract Vector3 OutlineWidth { get; }
        
        
        

        protected virtual void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            if (!TryGetComponent(out interactable))
            {
                Debug.LogError($"InteractableOutline on GO {gameObject.name} couldn't find IInteractable component. Outline is disabled.");
                var single = meshRenderer.materials[0];
                meshRenderer.materials = new[] { single };
                enabled = false;
                return;
            }

            SetOutlineWidth();
            SubEvents();
        }

        public void SetEnabled(bool isEnabled)
        {
            SetColor(interactable.OutlineColor);
            materialPropertyBlock.SetFloat(GlobalDefinitions.PropertyOutlineEnabled, isEnabled ? 1f : 0f);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        private void SetOutlineWidth()
        {
            materialPropertyBlock.SetVector(GlobalDefinitions.PropertyOutlineWidth, OutlineWidth);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        private void SetColor(Vector4 color)
        {
            if(color.Equals(currentColor)) return;
            currentColor = color;
            materialPropertyBlock.SetVector(GlobalDefinitions.PropertyOutlineColor, color);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        protected virtual void SubEvents()
        {
            ItemPicker.OnItemPickedUp += OnInteractionStart;
            ItemPicker.OnItemPickedDown += OnInteractionEnd;
            InteractionManager.OnInteractableDragStart += OnInteractionStart;
            InteractionManager.OnInteractableDragEnd += OnInteractionEnd;
            AbilityCaster.OnAbilityCastEnd += OnInteractionEnd;
            AbilityCaster.OnAbilityCastStart += OnInteractionStart;
            interactable.OnDestroyed += OnDestroyed;
        }

        protected virtual void UnsubEvents(IInteractable target)
        {
            ItemPicker.OnItemPickedUp -= OnInteractionStart;
            ItemPicker.OnItemPickedDown -= OnInteractionEnd;
            InteractionManager.OnInteractableDragStart -= OnInteractionStart;
            InteractionManager.OnInteractableDragEnd -= OnInteractionEnd;
            AbilityCaster.OnAbilityCastEnd -= OnInteractionEnd;
            AbilityCaster.OnAbilityCastStart -= OnInteractionStart;
            target.OnDestroyed -= OnDestroyed;
        }

        private void OnDestroyed(IInteractable target)
        {
            UnsubEvents(target);
            SetEnabled(false);
        }
        
        private void OnInteractionStart() => SetEnabled(false);

        private void OnInteractionEnd() => SetEnabled(interactable.CanInteract);
    }
}