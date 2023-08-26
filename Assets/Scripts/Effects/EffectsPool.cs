using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public abstract class EffectsPool<T>  : MonoBehaviour, IEffectsPool where T : EffectObject
    {
        [SerializeField] private T prefab;
        
        private readonly Stack<T> objectStack = new();

        

        // IEffectsPool
        public EffectObject GetEffectObject()
        {
            bool popped = objectStack.TryPop(out T pop);
            T obj = popped ? pop : Instantiate(prefab);
            if(popped) obj.OnTakenFromPool();
            else obj.ObjectPool = this;
            return obj; 
        }

        public void Pool(EffectObject effectObject)
        {
            if(effectObject is not T obj) return;
            objectStack.Push(obj);
            obj.OnPool();
        }

        public bool IsForEffect<TJ>() where TJ : EffectObject
        {
            return typeof(T) == typeof(TJ);
        }
    }
}