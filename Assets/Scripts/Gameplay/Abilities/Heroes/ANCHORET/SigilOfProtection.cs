using UnityEngine;

namespace Gameplay.Abilities
{
    public class SigilOfProtection : TargetBuffAbility
    {
        [SerializeField] private ParticleSystem castParticles;

        protected override ParticleSystem CastParticles => castParticles;
    }
}