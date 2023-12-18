using UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;
using Util.Enums;

namespace UI.Elements
{
    public class AttackTypeIndicator : TextTooltipProvider<AttackType>
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] sprites;

        public void SetAttackType(AttackType attackType)
        {
            LastValue = attackType;
            image.sprite = sprites[(int) attackType];
        }

        protected override string GetTooltipText()
        {
            return LastValue switch
            {
                AttackType.Melee => $"Attack type: {LastValue}\nCan only attack targets on the same location",
                AttackType.Ranged => $"Attack type: {LastValue}\nCan only attack targets on neighbour locations",
                AttackType.Magic => $"Attack type: {LastValue}\nCan attack targets on the same and neighbour locations",
                _ => ""
            };
        }
    }
}