using Gameplay.Cards;
using UnityEngine;

namespace Util.Dice
{
    [System.Serializable]
    public class EvaluatorPair
    {
        [SerializeField] private BaseEvaluator evaluator;
        [SerializeField] private CardAction cardAction;

        public BaseEvaluator Evaluator => evaluator;
        public CardAction Action => cardAction;
        
        public string Description => $"<b>{evaluator.Description}</b>: {cardAction.Description}\\n";
    }
}