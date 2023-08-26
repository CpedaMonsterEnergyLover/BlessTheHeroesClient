using System.Linq;
using UnityEngine;

namespace Util.Dice
{
    [System.Serializable]
    public class Dice
    {
        [SerializeField] private int[] values;
        [SerializeField] private int[] energy;

        public int[] Values => values.ToArray();
        public int[] Energy => energy.ToArray();

        public Dice()
        {
            values = new int[6];
            energy = new int[6];
        }
    }
}