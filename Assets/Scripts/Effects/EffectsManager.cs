using System.Linq;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(IEffectsPool))]
    public class EffectsManager : MonoBehaviour
    {
        private static IEffectsPool[] pools;

        
        
        private void Awake()
        {
            pools = GetComponents<IEffectsPool>();
        }

        public static T GetEffect<T>() where T : EffectObject
        {
            T obj = (T) pools.First(pool => pool.IsForEffect<T>()).GetEffectObject();
            return obj;
        }
    }
}