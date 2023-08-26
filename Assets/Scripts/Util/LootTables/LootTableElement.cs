using UnityEngine;

namespace Util.LootTables
{
    [System.Serializable]
    public class LootTableElement
    {
        [SerializeField] private Scriptable.Item item;
        [SerializeField, Range(0, 1)] private float value;

        public Scriptable.Item Item => item;
        
        public float Value
        {
            get => value;
            set => this.value = value;
        }
    }
}