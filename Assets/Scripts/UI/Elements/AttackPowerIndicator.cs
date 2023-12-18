using UI.Browsers;
using UI.Tooltips;
using UnityEngine;

namespace UI.Elements
{
    public class AttackPowerIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            int attackPower = TokenBrowser.SelectedToken is null ? 0 : TokenBrowser.SelectedToken.AttackPower;
            return $"Attack power / ATK\nAttack dice throws are {(attackPower < 0 ? "weakened" : "empowered")} by {Mathf.Abs(attackPower)}";
        }
    }
}