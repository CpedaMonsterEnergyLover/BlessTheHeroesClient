using System;
using System.Collections.Generic;
using System.Text;
using CardAPI;
using UnityEngine;

namespace Util.Dice
{
    [Serializable]
    public class EvaluatorSet
    {
        [SerializeReference] private List<UniversalDiceEvaluator> evaluators = new();
        [SerializeReference] private List<CardAction> actions = new ();
        
        public void AddEvaluator(UniversalDiceEvaluator evaluator) => evaluators.Add(evaluator);
        public void AddAction(CardAction action) => actions.Add(action);

        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < evaluators.Count; i++)
                {
                    sb
                        .Append("<b>")
                        .Append(evaluators[i].Description)
                        .Append("</b>")
                        .Append(": ")
                        .Append(actions[i].Description)
                        .Append("\n");
                }

                return sb.ToString();
            }
        }
        
        public bool Evaluate(int roll, out int result, out CardAction action)
        {
            result = -1;
            action = null;
            for (var i = 0; i < evaluators.Count; i++)
            {
                if (evaluators[i].Evaluate(roll, out result))
                {
                    action = i > actions.Count ? null : actions[i];
                    return action is not null;
                }
            }
            return false;
        }
    }
}