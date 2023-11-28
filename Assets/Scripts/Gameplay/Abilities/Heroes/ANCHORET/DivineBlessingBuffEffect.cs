using Gameplay.BuffEffects;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class DivineBlessingBuffEffect : RestorationBuffEffect
    {
        [SerializeField] private int maxHealthBonus;
        [SerializeField] private int maxManaBonus;

        protected override void OnApplied()
        {
            Manager.Token.AddMaxHealth(maxHealthBonus);
            Manager.Token.AddMaxMana(maxManaBonus);
        }

        protected override void OnRemoved()
        {
            Manager.Token.AddMaxHealth(-maxHealthBonus);
            Manager.Token.AddMaxMana(-maxManaBonus);
        }
    }
}