using UnityEngine;

namespace Gameplay.Tokens.Buffs.Effects
{
    public class DivineBlessing : BuffEffect
    {
        [SerializeField] private int maxHealthBonus;
        [SerializeField] private int manaRegen;

        protected override void OnApplied()
        {
            Manager.Token.AddMaxHealth(maxHealthBonus);
        }

        protected override void OnRemoved()
        {
            Manager.Token.AddMaxHealth(-maxHealthBonus);
        }

        protected override void OnTick()
        {
            Manager.Token.ReplenishMana(manaRegen);
        }
    }
}