using Gameplay.BuffEffects;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class WrathOfTheWitchBuffEffect : DamageOverTimeBuffEffect
    {
        protected virtual Sprite OverrideDamageSprite => null;
    }
}