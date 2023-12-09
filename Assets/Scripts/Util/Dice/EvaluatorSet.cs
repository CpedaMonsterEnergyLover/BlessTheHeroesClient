using System;
using System.Collections.Generic;
using System.Text;
using Gameplay.Cards;
using UnityEngine;

namespace Util.Dice
{
    [Serializable]
    public class EvaluatorSet
    {
        [SerializeField] private List<EvaluatorPair> evaluators = new();
        
        

        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var e in evaluators) sb.Append(e.Description);
                return sb.ToString();
            }
        }
        
        public bool Evaluate(int roll, out int result, out CardAction action)
        {
            result = -1;
            action = null;
            foreach (var pair in evaluators)
            {
                if (!pair.Evaluator.Evaluate(roll, out result)) continue;
                action = pair.Action;
                return true;
            }
            
            return false;
        }
    }
}