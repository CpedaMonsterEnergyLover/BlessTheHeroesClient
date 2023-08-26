using Gameplay.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera)),
    RequireComponent(typeof(PhysicsRaycaster))]
    public class MainCamera : MonoBehaviour
    {
        public static MainCamera Instance { get; private set; }
        public static UnityEngine.Camera Camera { get; private set; }
        public static PhysicsRaycaster InteractionRaycaster { get; private set; }
        
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        [SerializeField] private float zoomStep;
        [SerializeField] private Vector3 offset = new(0, 0, -1.5f);
        
        private Vector3 dragOrigin;
        private Vector3 minPos;
        private Vector3 maxPos;

        
        
        // Constructor
        private MainCamera() => Instance = this;
        
        // Unity methods
        private void Awake()
        {
            Camera = GetComponent<UnityEngine.Camera>();
            InteractionRaycaster = GetComponent<PhysicsRaycaster>();
            InteractionManager.OnCameraDragStart += OnCameraDragStart;
            InteractionManager.OnCameraDrag += OnCameraDrag;
            InteractionManager.OnCameraDragEnd += OnCameraDragEnd;
        }

        private void Update()
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel == 0f) return;

            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y + wheel * zoomStep * -1, minZoom, maxZoom);
            transform.position = pos;
        }
        
        
        
        // Class methods
        public void SetPosition(Vector3 pos) => transform.position = new Vector3(pos.x, transform.position.y, pos.z) + offset;
        
        private void OnCameraDragStart(Vector3 mousePos)
        {
            dragOrigin = ScreenToWorldPos(mousePos);
            enabled = false;
        }

        private void OnCameraDrag(Vector3 mousePos)
        {
            Vector3 pos = transform.position;
            Vector3 mouseWorldPos = ScreenToWorldPos(mousePos);
            Vector3 diff = dragOrigin - mouseWorldPos;
            pos = new Vector3(
                Mathf.Clamp(pos.x + diff.x, minPos.x, maxPos.x),
                pos.y,
                Mathf.Clamp(pos.z + diff.z, minPos.z, maxPos.z));
            transform.position = pos;
        }

        private void OnCameraDragEnd(Vector3 _)
        {
            enabled = true;
        }
        
        public Vector3 GetMousePosition()
        {
            Vector3 pos = Input.mousePosition;
            pos.z = transform.position.y;
            return pos;
        }


        public Vector3 GetMouseWorldPos() => ScreenToWorldPos(GetMousePosition());
        private static Vector3 ScreenToWorldPos(Vector3 mousePos) 
            => Camera.ScreenToWorldPoint(mousePos);

        public void SetMinPoint(Vector3 point) => minPos = point + offset;
        public void SetMaxPoint(Vector3 point) => maxPos = point + offset;
        
        
        
        // Util
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(minPos, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(maxPos, 0.1f);
        }
    }
}


