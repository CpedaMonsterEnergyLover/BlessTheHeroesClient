using Gameplay.GameCycle;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Util.Tokens
{
    public class TokenOutline : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        private MaterialPropertyBlock materialPropertyBlock;
        private static readonly int PropertyOutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
        private static readonly int PropertyOutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int PropertyOutlineWidth = Shader.PropertyToID("_OutlineWidth");

        private short currentColorID = -1;
        private IInteractable interactable;

        
        
        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            if (!TryGetComponent(out interactable))
            {
                Debug.LogError($"TokenOutline on GO {gameObject.name} couldn't find IInteractable component. Outline is disabled");
                var single = meshRenderer.materials[0];
                meshRenderer.materials = new[] { single };
                enabled = false;
            }
        }

        public void SetOutlineWidth(Vector3 width)
        {
            materialPropertyBlock.SetVector(PropertyOutlineWidth, width);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        public void SetEnabled(bool isEnabled)
        {
            switch (interactable)
            {
                case IControllableToken controllable:
                    if(controllable.ActionPoints == 0)
                        SetColor(GlobalDefinitions.TokenOutlineYellowColor, 1);
                    else 
                        SetColor(GlobalDefinitions.TokenOutlineGreenColor, 0);
                    break;
                case IUncontrollableToken:
                    if(TurnManager.CurrentStage is TurnStage.PlayersTurn)
                       SetColor(GlobalDefinitions.TokenOutlineGreenColor, 0); 
                    else 
                        SetColor(GlobalDefinitions.TokenOutlineRedColor, 2);
                    break;
            }
            materialPropertyBlock.SetFloat(PropertyOutlineEnabled, isEnabled ? 1f : 0f);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }

        private void SetColor(Vector4 color, short colorID)
        {
            if(currentColorID == colorID) return;
            currentColorID = colorID;
            materialPropertyBlock.SetVector(PropertyOutlineColor, color);
            meshRenderer.SetPropertyBlock(materialPropertyBlock, 1);
        }
    }
}