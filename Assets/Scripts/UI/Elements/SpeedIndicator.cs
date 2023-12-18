using UI.Browsers;
using UI.Tooltips;

namespace UI.Elements
{
    public class SpeedIndicator : TextTooltipProvider<int>
    {
        protected override string GetTooltipText()
        {
            int speed = TokenBrowser.SelectedToken is null ? 0 : TokenBrowser.SelectedToken.Speed;
            return $"Speed / SPD\nGains {speed} MOV at the start of each turn or by trading 1 ACT\nAutomatically passes flee check on locations with less than or equal to {speed} enemies";
        }
    }
}