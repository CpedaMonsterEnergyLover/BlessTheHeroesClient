using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.BuffEffects
{
    public class StackableStatBuffEffect : StackableBuffEffect
    {
        [SerializeField] private int defense;
        [SerializeField] private int speed;
        [SerializeField] private int attackPower;
        [SerializeField] private int maxHealth;
        [SerializeField] private int maxMana;
        [SerializeField] private int spellPower;
        
        protected override void OnRemoved()
        {
            IToken token = Manager.Token;
            token.AddDefense(- Stacks * defense);
            token.AddSpeed(- Stacks * speed);
            token.AddAttackPower(- Stacks * attackPower);
            token.AddMaxHealth(- Stacks * maxHealth);
            token.AddMaxMana(- Stacks * maxMana);
            token.AddSpellPower(- Stacks * spellPower);
            base.OnRemoved();
        }

        protected override void OnStacksChanged(int previousStacks, int newStacks)
        {
            IToken token = Manager.Token;
            token.AddDefense(newStacks * defense - previousStacks * defense);
            token.AddSpeed(newStacks * speed - previousStacks * speed);
            token.AddAttackPower(newStacks * attackPower - previousStacks * attackPower);
            token.AddMaxHealth(newStacks * maxHealth - previousStacks * maxHealth);
            token.AddMaxMana(newStacks * maxMana - previousStacks * maxMana);
            token.AddSpellPower(newStacks * spellPower - previousStacks * spellPower);
        }

        protected override void OnTick()
        {
        }
    }
}