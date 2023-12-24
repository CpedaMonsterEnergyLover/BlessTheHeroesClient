using UnityEngine;

namespace Pooling
{
    public abstract class Poolable : MonoBehaviour
    {
        public IObjectPool ObjectPool { get; set; }


        public abstract void OnPool();

        public abstract void OnTakenFromPool();
        
        public void Pool() => ObjectPool.Pool(this);

        public void SetPosition(Vector3 position) => transform.position = position;

        public void SetRotation(Quaternion rotation) => transform.rotation = rotation;
    }
}