using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.BuffEffects
{
    public class StatBuffEffect : BuffEffect
    {
        [SerializeField] private int defense;
        [SerializeField] private int speed;
        [SerializeField] private int attackPower;
        [SerializeField] private int maxHealth;
        [SerializeField] private int maxMana;
        [SerializeField] private int spellPower;
        
        protected override void OnApplied()
        {
            IToken token = Manager.Token;
            token.AddDefense(defense);
            token.AddSpeed(speed);
            token.AddAttackPower(attackPower);
            token.AddMaxHealth(maxHealth);
            token.AddMaxMana(maxMana);
            token.AddSpellPower(spellPower);
        }

        protected override void OnRemoved()
        {
            IToken token = Manager.Token;
            token.AddDefense(-defense);
            token.AddSpeed(-speed);
            token.AddAttackPower(-attackPower);
            token.AddMaxHealth(-maxHealth);
            token.AddMaxMana(-maxMana);
            token.AddSpellPower(-spellPower);
        }

        protected override void OnTick()
        {
        }
    }
}