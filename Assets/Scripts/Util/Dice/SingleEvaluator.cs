using UnityEngine;

namespace Util.Dice
{
    [CreateAssetMenu(menuName = "Evaluator/Single")]
    public class SingleEvaluator : BaseEvaluator
    {
        [Header("Single Evaluator")]
        [SerializeField, Range(1, 6)] private int targetRoll;
        
        public override bool Evaluate(int roll, out int result)
        {
            result = roll;
            return roll == targetRoll;
        }

        public override string Description => $"D{targetRoll}";
    }
}