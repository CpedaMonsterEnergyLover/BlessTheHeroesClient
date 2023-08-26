using UnityEngine;

namespace Util
{
    [System.Serializable]
    public class LootTable
    {
        
    }

    [System.Serializable]
    public struct LootTableElement
    {
        [SerializeField] private Scriptable.Item item;
    }
}