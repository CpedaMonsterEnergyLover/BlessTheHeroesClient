using TMPro;
using UI.Tooltips;
using UnityEngine;

namespace UI.Elements
{
    public class CreatureAttackIndicator : TextTooltipProvider<int>
    {
        [SerializeField] private TMP_Text text;

        public void SetAttackDiceAmount(int amount)
        {
            LastValue = amount;
            text.SetText(amount.ToString());
        }

        protected override string GetTooltipText()
        {
            return $"Throws {LastValue} attack dice{(LastValue != 1 ? "s" : string.Empty)} when performing a physical attack";
        }
    }
}