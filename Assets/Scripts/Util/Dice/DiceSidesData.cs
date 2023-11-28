using System.Linq;
using UnityEngine;

namespace Util.Dice
{
    [System.Serializable]
    public class DiceSidesData
    {
        [SerializeField] private int[] values;
        [SerializeField] private int[] energy;

        public int[] Values => values.ToArray();
        public int[] Energy => energy.ToArray();

        public DiceSidesData()
        {
            values = new int[6];
            energy = new int[6];
        }
    }
}