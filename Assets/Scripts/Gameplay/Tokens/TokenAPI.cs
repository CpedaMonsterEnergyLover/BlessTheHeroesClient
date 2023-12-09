using Cysharp.Threading.Tasks;
using Gameplay.Aggro;
using UI;
using UnityEngine;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public abstract partial class Token<T, TJ>
    {
        public event IToken.TokenResourceEvent OnDamaged;
        public event IToken.TokenResourceEvent OnHealed;
        public event IToken.TokenResourceEvent OnManaReplenished;
        public event IToken.TokenEvent OnDeath;
        public event IToken.TokenMoveEvent OnMove;
        public event IToken.TokenDamageAbsorbtionEvent OnDamageAbsorbed;

        
        
        public void AddMaxHealth(int amount)
        {
            if(amount == 0) return;
            maxHealthBonus += amount;
            SetHealth(Mathf.Clamp(CurrentHealth + amount, 1, MaxHealth));
        }


        public void AddMaxMana(int amount)
        {
            if(amount == 0) return;
            maxManaBonus += amount;
            SetMana(Mathf.Clamp(CurrentMana + amount, 0, MaxMana));
        }

        public void AddAttackPower(int amount)
        {
            if(amount == 0) return;
            attackPower += amount;
            InvokeDataChangedEvent();
        }

        public void AddDefense(int amount)
        {
            if(amount == 0) return;
            defense += amount;
            InvokeDataChangedEvent();
        }

        public void AddSpellPower(int amount)
        {
            if(amount == 0) return;
            spellPower += amount;
            InvokeDataChangedEvent();
        }
        
        public void AddSpeed(int amount)
        {
            if(amount == 0) return;
            speedBonus += amount;
            InvokeDataChangedEvent();
        }
        
        public void Heal(int health, IAggroManager aggroReceiver = null)
        {
            if(Dead) return;
            int raw = CurrentHealth + health;
            int clamped = Mathf.Clamp(raw, CurrentHealth, MaxHealth);
            if(aggroReceiver is not null) aggroReceiver.AddAggro(health - (raw - clamped), this);
            SetHealth(clamped);
            damageAnimator.PlayHealing(health).Forget();
            OnHealed?.Invoke(health);
        }

        public void ReplenishMana(int mana)
        {
            if(MaxMana == 0) return;
            SetMana(Mathf.Clamp(CurrentMana + mana, CurrentMana, MaxMana));
            OnManaReplenished?.Invoke(mana);
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
            OnActionsChanged?.Invoke(this);
        }
        
        public bool DrainMana(int amount)
        {
            if (amount > CurrentMana) return false;
            SetMana(CurrentMana - amount);
            return true;
        }
        
        public void SetHealth(int value)
        {
            CurrentHealth = value;
            healthText.SetText(value.ToString());
            OnHealthChanged?.Invoke(this);
        }

        public void SetMana(int value)
        {
            CurrentMana = value;
            OnManaChanged?.Invoke(this);
        }
    }
}