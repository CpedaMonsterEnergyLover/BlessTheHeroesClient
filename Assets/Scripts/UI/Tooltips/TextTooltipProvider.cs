namespace UI.Tooltips
{
    public abstract class TextTooltipProvider<T> : TooltipProvider<T>
    {
        protected abstract string GetTooltipText();
        
        protected override void ShowTooltip()
        {
            TextTooltip.Instance.SetText(GetTooltipText());
        }

        protected override void HideTooltip()
        {
            TextTooltip.Instance.Hide();
        }
    }
}