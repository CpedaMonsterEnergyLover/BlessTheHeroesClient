using UI.Browsers;
using UI.Tooltips;
using UnityEngine;

namespace UI.Elements
{
    public class DefenseIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            int defense = TokenBrowser.SelectedToken is null ? 0 : TokenBrowser.SelectedToken.Defense;
            return $"Defense / DEF\nDefense dice throws are {(defense < 0 ? "weakened" : "empowered")} by {Mathf.Abs(defense)}";
        }
    }
}