using UnityEngine;

namespace Util.Dice
{
    [System.Serializable]
    public class RangeEvaluator : UniversalDiceEvaluator
    {
        [Header("Range Evaluator")]
        [SerializeField] private int from;
        [SerializeField] private int to;


        
        public override bool Evaluate(int roll, out int result)
        {
            result = roll;
            return roll >= from && roll <= to;
        }

        public override string Description => $"D{from}-{to}";
    }
}