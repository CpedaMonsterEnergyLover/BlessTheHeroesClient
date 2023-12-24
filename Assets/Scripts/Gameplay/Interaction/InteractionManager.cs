using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Camera;
using Cysharp.Threading.Tasks;
using UI.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private int doubleClickTresholdMS = 500;
        [SerializeField] private float dragTimeTreshold;
        [SerializeField] private float dragDistanceTreshold;
        [SerializeField] private InteractionTooltip interactionTooltip;

        private bool dragInitialized;
        private bool dragStartOnUI;
        private float dragThresholdTimer;
        private Vector2 dragOrigin;
        private IInteractable clickedInteractable;
        
        private static UniTask clickTask;
        private static bool interacting;
        private static readonly List<IInteractable> TargetsCache = new();
        public static bool Dragging { get; private set; }

        public static bool AnyInteractionActive
            => Dragging || InspectionManager.Inspecting || AbilityCaster.IsDragging || ItemPicker.IsPicked; 

        public delegate void CameraDragEvent(Vector3 mousePos = default);
        public static event CameraDragEvent OnCameraDragStart;
        public static event CameraDragEvent OnCameraDrag;
        public static event CameraDragEvent OnCameraDragEnd;

        public delegate void InteractableDragEvent();
        public static event InteractableDragEvent OnInteractableDragStart;
        public static event InteractableDragEvent OnInteractableDragEnd;

        
        

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
                TargetsCache.Clear();
                interactableOnDrag.GetInteractionTargets(TargetsCache);
                UpdateOutlinesOnCastStart();
                interacting = true;
            }
            else
            {
                interacting = false;
                clickedInteractable = null;
                OnCameraDragStart?.Invoke(MainCamera.Instance.GetMousePosition());
            }
        }

        private void Drag()
        {
            if(dragStartOnUI) return;
            if(!interacting) OnCameraDrag?.Invoke(MainCamera.Instance.GetMousePosition());
            else if(clickedInteractable is IInteractableOnDrag interactableOnDrag) 
                interactionTooltip.Show(interactableOnDrag.OnDrag(GetInteractionResult()));
        }

        private void CancelDrag()
        {
            if (dragStartOnUI) return;
            if (!Dragging)
            {
                if (clickedInteractable is IInteractableOnClick {CanClick: true} interactableOnClick)
                {
                    interactableOnClick.OnClick(GetInteractionResult(), clickTask.Status is UniTaskStatus.Pending ? 2 : 1);
                    clickTask = ClickTask();
                }
            }
            else if(clickedInteractable is IInteractableOnDrag interactableOnDrag)
            {
                interactableOnDrag.OnDragEnd(GetInteractionResult());
                UpdateOutlinesOnCastEnd();
                interactionTooltip.Show(null);
            }

            interacting = false;
            clickedInteractable = null;
            Dragging = false;
            dragInitialized = false;
            OnCameraDragEnd?.Invoke();
        }
        
        public static InteractionResult GetInteractionResult()
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
        
        private static void UpdateOutlinesOnCastStart()
        {
            OnInteractableDragStart?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.EnableOutline();
        }
        
        private static void UpdateOutlinesOnCastEnd()
        {
            OnInteractableDragEnd?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.UpdateOutline();
        }

        private async UniTask ClickTask()
        {
            await UniTask.Delay(doubleClickTresholdMS);
        }
    }
}