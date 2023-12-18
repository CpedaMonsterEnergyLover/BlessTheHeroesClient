using TMPro;
using UI.Tooltips;
using UnityEngine;
using Util.Enums;

namespace UI.Elements
{
    public class ArmorTypeIndicator : TextTooltipProvider<ArmorType>
    {
        [SerializeField] private TMP_Text text;

        public void SetArmorType(ArmorType armorType)
        {
            LastValue = armorType;
            text.SetText(armorType.ToString()[..1]);
        }

        protected override string GetTooltipText()
        {
            return LastValue == 0 
                ? $"Armor type: {LastValue}\nCannot throw defence dice"
                : $"Armor type: {LastValue}\nThrows {(int)LastValue} defence dice";
        }
    }
}