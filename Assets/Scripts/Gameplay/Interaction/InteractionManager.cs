using Camera;
using UI.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float dragTimeTreshold;
        [SerializeField] private float dragDistanceTreshold;
        [SerializeField] private InteractionTooltip interactionTooltip;

        private bool dragInitialized;
        private float dragThresholdTimer;
        private Vector2 dragOrigin;
        private IInteractable clickedInteractable;
        private bool dragStartOnUI;


        public static bool Dragging { get; private set; }
        public static bool Interacting { get; private set; }

        public delegate void InteractionManagerEvent(Vector3 mousePos = default);
        public static event InteractionManagerEvent OnCameraDragStart;
        public static event InteractionManagerEvent OnCameraDrag;
        public static event InteractionManagerEvent OnCameraDragEnd;
        
        

        private void Update()
        {
            bool pressed = dragInitialized ? Input.GetMouseButtonUp(0) : Input.GetMouseButtonDown(0);
            
            if (pressed)
            {
                if (dragInitialized)
                    CancelDrag();
                else
                    InitializeDrag();
            } 
            else if (Dragging)
                Drag();
            else if (dragInitialized && CanStartDrag()) 
                StartDrag();
        }

        private bool CanStartDrag()
        {
            dragThresholdTimer += Time.deltaTime;
            float dist = Vector2.Distance(dragOrigin, MainCamera.Instance.GetMousePosition());
            return dist >= dragDistanceTreshold || dragThresholdTimer >= dragTimeTreshold;
        }
        
        private void InitializeDrag()
        {
            dragStartOnUI = EventSystem.current.IsPointerOverGameObject();
            if(dragStartOnUI) return;
            
            clickedInteractable = GetInteractionResult().Target;
            dragInitialized = true;
            dragOrigin = MainCamera.Instance.GetMousePosition();
            dragThresholdTimer = 0f;
        }

        private void StartDrag()
        {
            if(dragStartOnUI) return;
            Dragging = true;
            if (clickedInteractable is IInteractableOnDrag {CanInteract: true} interactableOnDrag)
            {
                interactableOnDrag.OnDragStart(GetInteractionResult());
                Interacting = true;
            }
            else
            {
                Interacting = false;
                clickedInteractable = null;
                OnCameraDragStart?.Invoke(MainCamera.Instance.GetMousePosition());
            }
        }

        private void Drag()
        {
            if(dragStartOnUI) return;
            if(!Interacting) OnCameraDrag?.Invoke(MainCamera.Instance.GetMousePosition());
            else if(clickedInteractable is IInteractableOnDrag interactableOnDrag) 
                interactionTooltip.Show(interactableOnDrag.OnDrag(GetInteractionResult()));
        }

        private void CancelDrag()
        {
            if(dragStartOnUI) return;
            if (!Dragging)
            {
                if (clickedInteractable is IInteractableOnClick {CanClick: true} interactableOnClick)
                    interactableOnClick.OnClick(GetInteractionResult());
            }
            else if(clickedInteractable is IInteractableOnDrag interactableOnDrag)
            {
                interactableOnDrag.OnDragEnd(GetInteractionResult());
                interactionTooltip.Show(null);
            }

            Interacting = false;
            clickedInteractable = null;
            Dragging = false;
            dragInitialized = false;
            OnCameraDragEnd?.Invoke();
        }
        
        public InteractionResult GetInteractionResult()
        {
            if(EventSystem.current.IsPointerOverGameObject() || 
               !Physics.Raycast(
                   MainCamera.Camera.ScreenPointToRay(MainCamera.Instance.GetMousePosition()), 
                   out var info,
                   50f, layerMask: 1 << 6)) return new InteractionResult(false);

            return info.collider.TryGetComponent(out InteractionCollider interactionCollider) ?
                new InteractionResult(true, interactionCollider.Target, info.point) :
                new InteractionResult(false);
        }
    }
}