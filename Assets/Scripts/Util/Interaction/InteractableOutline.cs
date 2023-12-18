﻿using Gameplay.Interaction;
using UnityEngine;

namespace Util.Interaction
{
    public class InteractableOutline : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        private MaterialPropertyBlock materialPropertyBlock;
        private static readonly int PropertyOutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
        private static readonly int PropertyOutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int PropertyOutlineWidth = Shader.PropertyToID("_OutlineWidth");

        private Vector4 currentColor = Vector4.zero;
        protected IInteractable Interactable { get; private set; }

        
        
        protected virtual void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            if (!TryGetComponent(out IInteractable interactable))
            {
                Debug.LogError($"InteractableOutline on GO {gameObject.name} couldn't find IInteractable component. Outline is disabled.");
                var single = meshRenderer.materials[0];
                meshRenderer.materials = new[] { single };
                enabled = false;
                return;
            }

            Interactable = interactable;
        }

        public void SetOutlineWidth(Vector3 width)
        {
            materialPropertyBlock.SetVector(PropertyOutlineWidth, width);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        public void SetEnabled(bool isEnabled)
        {
            SetColor(Interactable.OutlineColor);
            materialPropertyBlock.SetFloat(PropertyOutlineEnabled, isEnabled ? 1f : 0f);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        private void SetColor(Vector4 color)
        {
            if(color.Equals(currentColor)) return;
            currentColor = color;
            materialPropertyBlock.SetVector(PropertyOutlineColor, color);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }
    }
}