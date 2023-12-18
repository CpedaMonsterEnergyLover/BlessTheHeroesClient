using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class ActionIndicator : TextTooltipProvider<int>
    {
        [SerializeField] private Image[] indicators;

        public void SetActions(int amount)
        {
            LastValue = amount;
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].enabled = i < amount;
            }
        }

        protected override string GetTooltipText()
        {
            return $"Actions / ACT\nCan perform up to {LastValue} actions this turn";
        }
    }
}