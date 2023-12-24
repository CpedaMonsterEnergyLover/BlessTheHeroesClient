using System.Linq;
using UnityEngine;

namespace Pooling
{
    [RequireComponent(typeof(IObjectPool))]
    public class PoolManager : MonoBehaviour
    {
        private static IObjectPool[] pools;

        
     
        private void Awake()
        {
            pools = GetComponents<IObjectPool>();
        }

        public static T GetEffect<T>() where T : Poolable
        {
            T obj = (T) pools.First(pool => pool.IsForEffect<T>()).GetEffectObject();
            return obj;
        }
    }
}