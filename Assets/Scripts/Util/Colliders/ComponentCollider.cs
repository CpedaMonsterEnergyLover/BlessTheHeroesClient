using UnityEngine;

namespace Util.Colliders
{
    [RequireComponent(typeof(Collider))]
    public abstract class ComponentCollider<T> : MonoBehaviour
    {
        [SerializeField] private Component component;

        public T Target { get; private set; }
        
        private void Awake()
        {
            if (component is not T target)
            {
                
#if UNITY_EDITOR
                Debug.LogError($"Provided component {component.name} is not a type of {typeof(T)}," +
                               $"which required by {GetType()} on GameObject {name}");
                GetComponent<Collider>().enabled = false;
#endif

                enabled = false;
            } else Target = target;
        }
    }
}