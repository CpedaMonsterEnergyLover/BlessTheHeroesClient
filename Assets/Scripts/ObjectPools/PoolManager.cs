using System.Linq;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(IObjectPool))]
    public class PoolManager : MonoBehaviour
    {
        private static IObjectPool[] pools;

        
     
        private void Awake()
        {
            pools = GetComponents<IObjectPool>();
        }

        public static T GetEffect<T>() where T : PoolObject
        {
            T obj = (T) pools.First(pool => pool.IsForEffect<T>()).GetEffectObject();
            return obj;
        }
    }
}