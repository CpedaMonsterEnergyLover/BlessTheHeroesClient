using UnityEngine;

namespace Util.Dice
{
    [CreateAssetMenu(menuName = "Evaluator/Range")]
    public class RangeEvaluator : BaseEvaluator
    {
        [Header("Range Evaluator")]
        [SerializeField, Range(1, 6)] private int from;
        [SerializeField, Range(1, 6)] private int to;


        
        public override bool Evaluate(int roll, out int result)
        {
            result = roll;
            return roll >= from && roll <= to;
        }

        public override string Description => $"D{from}-{to}";
    }
}