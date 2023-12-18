using Gameplay.Tokens;
using UI.Browsers;
using UI.Tooltips;

namespace UI.Elements
{
    public class MovementPointsIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            IToken selected = TokenBrowser.SelectedToken;
            return selected is HeroToken 
                ? $"Movement points / MOV\nCan move on or open up to {selected.MovementPoints} locations on this turn" 
                : $"Movement points / MOV\nCan move on up to {selected.MovementPoints} locations on this turn";
        }
    }
}