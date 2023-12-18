using Gameplay.Dice;
using Gameplay.Tokens;
using UI.Browsers;
using UI.Tooltips;

namespace UI.Elements
{
    public class EnergyIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            IToken selected = TokenBrowser.SelectedToken;
            
            return selected is HeroToken 
                ? $"Energy\nCan spend up to {EnergyManager.Instance.Energy} energy points this turn" 
                : "Energy\nCannot spend energy points";
        }
    }
}