using UI;

namespace Util.Tokens
{
    public interface IHasHealth
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; }

        
        
        public void SetHealth(int value)
        {
            CurrentHealth = value;
            if(TokenBrowser.Instance.SelectedToken == this)
                TokenBrowser.Instance.OnHealthChanged(CurrentHealth, MaxHealth);
        }
    }
}