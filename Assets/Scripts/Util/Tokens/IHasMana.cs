using UI;

namespace Util.Tokens
{
    public interface IHasMana
    {
        public int CurrentMana { get; set; }
        public int MaxMana { get; }


        
        public bool DrainMana(int amount)
        {
            if (amount > CurrentMana) return false;
            SetMana(CurrentMana - amount);
            return true;
        }
        
        public void SetMana(int value)
        {
            CurrentMana = value;
            if(TokenBrowser.Instance.SelectedToken == this)
                TokenBrowser.Instance.OnManaChanged(CurrentMana, MaxMana);
        }
    }
}