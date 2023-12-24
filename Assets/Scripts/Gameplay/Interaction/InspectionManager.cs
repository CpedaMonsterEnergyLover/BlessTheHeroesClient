using Camera;
using Gameplay.Cards;
using UI.Inspection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.Interaction
{
    public class InspectionManager : MonoBehaviour
    {
        [SerializeField] private Image inspectionIcon;
        [SerializeField] private InspectionTooltip inspectionTooltip;

        private IInteractable lastTarget;
        
        
        public static bool Inspecting { get; private set; }
        
        private void Update()
        {
            if(ItemPicker.IsPicked) return;
            bool pressed = Inspecting ? Input.GetMouseButtonUp(1) : Input.GetMouseButtonDown(1);
            
            if (pressed)
            {
                if (Inspecting)
                    CancelInspection();
                else
                    StartInspection();
            } else if(Inspecting) Inspect();
        }

        private void StartInspection()
        {
            Inspecting = true;
            inspectionIcon.gameObject.SetActive(!EventSystem.current.IsPointerOverGameObject());
        }
        
        private void CancelInspection()
        {
            Inspecting = false;
            inspectionIcon.gameObject.SetActive(false);
            lastTarget = null;
            inspectionTooltip.Toggle(false);
        }

        private void Inspect()
        {
            bool overUI = EventSystem.current.IsPointerOverGameObject();
            inspectionIcon.gameObject.SetActive(!overUI);
            if(overUI) return;

            bool hasTarget = GetInspectionTarget(out IInteractable interactable);
            if(lastTarget == interactable) return;
            
            if (hasTarget && 
                interactable is Card { IsOpened: true } card)
            {
                inspectionIcon.color = Color.green;
                lastTarget = interactable;
                InspectCard(card);
            }
            else
            {
                inspectionIcon.color = Color.white;
                lastTarget = null;
                inspectionTooltip.Toggle(false);
            }
        }

        private void InspectCard(Card card)
        {
            inspectionTooltip.InspectCard(card);
        }
        
        private bool GetInspectionTarget(out IInteractable interactable)
        {
            interactable = null;
            if(!Physics.Raycast(
                   MainCamera.Camera.ScreenPointToRay(MainCamera.Instance.GetMousePosition()), 
                   out var info,
                   50f, layerMask: 1 << 6)) return false;

            if(info.collider.TryGetComponent(out InteractionCollider interactionCollider))
                interactable = interactionCollider.Target;
            return interactionCollider is not null;
        }
    }
}