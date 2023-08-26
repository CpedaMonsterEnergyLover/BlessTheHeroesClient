using UI;
using UnityEngine;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public abstract partial class Token<T>
    {
        public void Heal(int damage)
        {
            ((IHasHealth) this).SetHealth(Mathf.Clamp(CurrentHealth + damage, CurrentHealth, MaxHealth));
        }
        
        public void SetMovementPoints(int amount)
        {
            MovementPoints = Mathf.Clamp(amount, 0, int.MaxValue);
            InvokeDataChangedEvent();
        }

        public void SetActionPoints(int amount)
        {
            ActionPoints = Mathf.Clamp(amount, 0, int.MaxValue);
            InvokeDataChangedEvent();
            if(this is HeroToken hero && ReferenceEquals(TokenBrowser.Instance.SelectedToken, this))
                TokenBrowser.Instance.OnActionChanged(hero);
        }
    }
}