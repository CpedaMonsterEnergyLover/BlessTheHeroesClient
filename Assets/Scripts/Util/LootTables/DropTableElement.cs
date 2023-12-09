using Scriptable;
using UnityEngine;

namespace Util.LootTables
{
    [System.Serializable]
    public class DropTableElement
    {
        [SerializeField] private LootTable lootTable;
        [SerializeField, Range(0, 1)] private float value;

        public bool DropItem(out Item item, float modifier = 1)
        {
            item = null;
            if (value >= 1f || Random.value <= value * modifier) 
                item = lootTable.GetRandomItem();

            return item is not null;
        }
    }
}