using UI.Browsers;
using UI.Tooltips;
using UnityEngine;

namespace UI.Elements
{
    public class SpellPowerIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            int spellPower = TokenBrowser.SelectedToken is null ? 0 : TokenBrowser.SelectedToken.SpellPower;
            return $"Spell power / SPELL\nSpell dice throws are {(spellPower < 0 ? "weakened" : "empowered")} by {Mathf.Abs(spellPower)}";
        }
    }
}