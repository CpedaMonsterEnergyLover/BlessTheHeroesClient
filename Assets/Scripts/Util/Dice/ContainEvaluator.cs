﻿using UnityEngine;

namespace Util.Dice
{
    [CreateAssetMenu(menuName = "Evaluator/Contain")]
    public class ContainEvaluator : BaseEvaluator
    {
        [Header("Contain Evaluator")]
        [SerializeField, Range(1, 6)] private int step;
        
        public override bool Evaluate(int roll, out int result)
        {
            result = roll % step;
            if (result == 0) result = step;
            return true;
        }

        public override string Description => $"D%{step}";
    }
}