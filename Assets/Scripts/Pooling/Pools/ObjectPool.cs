using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public abstract class ObjectPool<T>  : MonoBehaviour, IObjectPool where T : Poolable
    {
        [SerializeField] private T prefab;

        private readonly Stack<T> objectStack = new();

        
        
        public Poolable GetEffectObject()
        {
            bool popped = objectStack.TryPop(out T pop);
            T obj = popped ? pop : Instantiate(prefab);
            obj.OnTakenFromPool();
            if (popped) return obj;
            
            obj.ObjectPool = this;
            OnInstantiated(obj);
            return obj; 
        }

        protected virtual void OnInstantiated(T obj)
        {
        }

        public void Pool(Poolable effectObject)
        {
            if(effectObject is not T obj) return;
            objectStack.Push(obj);
            obj.OnPool();
        }

        public bool IsForEffect<TJ>() where TJ : Poolable
        {
            return typeof(T) == typeof(TJ);
        }
    }
}