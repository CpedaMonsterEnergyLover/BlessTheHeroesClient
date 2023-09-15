using UnityEngine;

namespace Util.Dice
{
    [System.Serializable]
    public class SingleEvaluator : EvaluatorBase
    {
        [Header("Single Evaluator")]
        [SerializeField] private int targetRoll;
        
        public override bool Evaluate(int roll, out int result)
        {
            result = roll;
            return roll == targetRoll;
        }

        public override string Description => $"D{targetRoll}";
    }
}