using Cysharp.Threading.Tasks;
using Gameplay.Aggro;
using UnityEngine;
using Util;
using Util.Enums;

namespace Gameplay.Tokens
{
    public abstract partial class Token<T, TJ>
    {
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
            if(aggroReceiver is not null)
            {
                int aggro = health - (raw - clamped);
                foreach (var creature in Card.Creatures) 
                    aggroReceiver.AddAggro(aggro, creature);
            }
            SetHealth(clamped);
            damageAnimator.PlayHealingAsync(health, GlobalDefinitions.HolyDamageType, DamageImpact.Normal,
                 aggroReceiver is null ? null : aggroReceiver.Bearer.TokenTransform).Forget();
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
            OnMovementPointsChanged?.Invoke(this);
            UpdateOutlineByCanInteract();
        }

        public void SetActionPoints(int amount)
        {
            ActionPoints = Mathf.Clamp(amount, 0, 4);
            OnActionsChanged?.Invoke(this);
            UpdateOutlineByCanInteract();
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

        protected void SetMana(int value)
        {
            CurrentMana = value;
            OnManaChanged?.Invoke(this);
        }
    }
}