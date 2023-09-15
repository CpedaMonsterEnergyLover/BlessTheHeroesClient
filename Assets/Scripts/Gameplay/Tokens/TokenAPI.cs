using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using Util.Tokens;

namespace Gameplay.Tokens
{
    public abstract partial class Token<T>
    {
        public void AddMaxHealth(int amount)
        {
            maxHealthBonus += amount;
            ((IHasHealth) this).SetHealth(Mathf.Clamp(CurrentHealth + amount, 1, MaxHealth));
        }

        public void AddMaxMana(int amount)
        {
            maxManaBonus += amount;
            ((IHasMana) this).SetMana(Mathf.Clamp(CurrentMana + amount, 0, MaxMana));
        }

        public void AddAttackPower(int amount)
        {
            attackPower += amount;
            InvokeDataChangedEvent();
        }

        public void AddDefense(int amount)
        {
            defense += amount;
            InvokeDataChangedEvent();
        }

        public void AddSpellPower(int amount)
        {
            spellPower += amount;
            InvokeDataChangedEvent();
        }
        
        public void AddSpeed(int amount)
        {
            speedBonus += amount;
            InvokeDataChangedEvent();
        }

        public void Damage(int damage, int delayMS = 200, Sprite overrideDamageSprite = null)
        {
            DamageAsync(damage, delayMS, overrideDamageSprite).Forget();
        }

        public void Heal(int health)
        {
            HealAsync(health).Forget();
        }

        public void ReplenishMana(int mana)
        {
            if(MaxMana == 0) return;
            ((IHasMana)this).SetMana(Mathf.Clamp(CurrentMana + mana, CurrentMana, MaxMana));
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
            TokenBrowser.Instance.OnActionsChanged(this);
        }
    }
}