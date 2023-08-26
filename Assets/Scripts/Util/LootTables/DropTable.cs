using System.Collections.Generic;
using UnityEngine;

namespace Util.LootTables
{
    [System.Serializable]
    public class DropTable
    {
        [SerializeField] private List<DropTableElement> content = new();
        [SerializeField, Range(0, 1)] private float coinsDropRate;
        [SerializeField] private Vector2Int coins;

        
        
        public int Coins
        {
            get
            {
                if (coinsDropRate == 0 || Random.value > coinsDropRate) return 0;
                return Random.Range(coins.x, coins.y + 1);
            }
        }

        public List<Scriptable.Item> Drop 
        {
            get {
                List<Scriptable.Item> items = new();
                foreach (DropTableElement element in content)
                {
                    if (element.DropItem(out Scriptable.Item item))
                        items.Add(item);
                }
                return items;
            }
        }
    }
}