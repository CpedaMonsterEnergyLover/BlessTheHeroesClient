using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Util.LootTables
{
    [CreateAssetMenu(menuName = "Loot Table")]
    public class LootTable : ScriptableObject
    {
        [SerializeField] private List<LootTableElement> content = new();



        public Scriptable.Item GetRandomItem()
        {
            if(content.Count == 0) return null;
            float rnd = Random.value;
            float step = 0;
            foreach (LootTableElement element in content)
            {
                if (rnd >= step + element.Value)
                {
                    step += element.Value;
                    continue;
                }

                return element.Item;
            }

            return null;
        }
        
        public void AddItem() => content.Add(new LootTableElement());

        public void Flatten()
        {
            int count = content.Count(element => element.Item is not null);
            if (count == 0) return;

            float each = 1f / count;
            foreach (LootTableElement element in content)
                element.Value = element.Item is null 
                    ? 0 
                    : Mathf.Clamp01(each);
        }

        public void OnValidate()
        {
            KeepUnique();
            KeepInBounds();
        }
        
        private void KeepInBounds()
        {
            float sum = content.Sum(e => e.Value);
            if (sum == 0) return;

            foreach (LootTableElement element in content)
            {
                element.Value = element.Item is null 
                    ? 0 
                    : Mathf.Clamp01(element.Value / sum);
            }
        }

        private void KeepUnique()
        {
            Dictionary<Scriptable.Item, int> counts = new();
            foreach (var key in content.Select(element => element.Item))
            {
                if(key is null) return;
                if (counts.ContainsKey(key)) counts[key]++;
                else counts[key] = 1;
            }
            
            foreach (var (item, count) in counts)
            {
                if(count == 1) continue;
                int temp = count;
                while (temp > 1)
                {
                    content.Remove(content.Last(element => element.Item == item));
                    temp--;
                }
                Debug.LogWarning($"Item {item.Name} was removed because it already was in a loot table.");
            }
        }
    }
}