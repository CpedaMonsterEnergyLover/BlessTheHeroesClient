using System.Text;
using Scriptable;
using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class CreatureTypeIndicator : TextTooltipProvider<CreatureType>
    {
        [SerializeField] private Image image;

        public void SetCreatureType(CreatureType creatureType)
        {
            LastValue = creatureType;
            image.sprite = creatureType.Icon;
        }
        
        
        protected override string GetTooltipText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Race: {LastValue.Title}");
            LastValue.AddResistances(sb);
            LastValue.AddVulnerabilities(sb);
            return sb.ToString();
        }
    }
}