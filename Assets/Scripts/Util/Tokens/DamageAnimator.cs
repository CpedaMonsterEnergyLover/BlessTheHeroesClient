using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Effects;
using Scriptable;
using UnityEngine;
using Util.Enums;

namespace Util.Tokens
{
    public class DamageAnimator : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private ParticleSystem natureParticles;
        [SerializeField] private ParticleSystem poisonParticles;
        [SerializeField] private ParticleSystem physicalParticles;
        [SerializeField] private ParticleSystem shadowParticles;
        [SerializeField] private ParticleSystem bloodParticles;
        [SerializeField] private ParticleSystem waterParticles;
        [SerializeField] private ParticleSystem arcaneParticles;
        [SerializeField] private ParticleSystem coldParticles;
        [SerializeField] private ParticleSystem holyParticles;
        [SerializeField] private ParticleSystem earthParticles;
        [SerializeField] private ParticleSystem airParticles;

        private readonly Dictionary<DamageType, ParticleSystem> damageToParticles = new();

        private void Start()
        {
            damageToParticles[GlobalDefinitions.FireDamageType] = fireParticles;
            damageToParticles[GlobalDefinitions.NatureDamageType] = natureParticles;
            damageToParticles[GlobalDefinitions.PoisonDamageType] = poisonParticles;
            damageToParticles[GlobalDefinitions.PhysicalDamageType] = physicalParticles;
            damageToParticles[GlobalDefinitions.ShadowDamageType] = shadowParticles;
            damageToParticles[GlobalDefinitions.BloodDamageType] = bloodParticles;
            damageToParticles[GlobalDefinitions.WaterDamageType] = waterParticles;
            damageToParticles[GlobalDefinitions.ArcaneDamageType] = arcaneParticles;
            damageToParticles[GlobalDefinitions.ColdDamageType] = coldParticles;
            damageToParticles[GlobalDefinitions.HolyDamageType] = holyParticles;
            damageToParticles[GlobalDefinitions.AirDamageType] = airParticles;
            damageToParticles[GlobalDefinitions.EarthDamageType] = earthParticles;
        }


        public void PlayDamage(int damage, DamageType damageType, DamageImpact impact, Transform sourceTransform = null)
            => PlayDamageAsync(damage, damageType, impact, sourceTransform).Forget();

        public void PlayHealing(int healing, DamageType damageType, DamageImpact impact, Transform sourceTransform = null)
            => PlayHealingAsync(healing, damageType, impact, sourceTransform).Forget();
        
        public async UniTask PlayDamageAsync(int damage, DamageType damageType, DamageImpact impact, Transform sourceTransform = null)
        {
            if(damageToParticles.ContainsKey(damageType)) damageToParticles[damageType].Play();
            await PoolManager.GetEffect<EffectText>().PlayDamage(transform, damage, damageType, impact, sourceTransform);
        }

        public async UniTask PlayHealingAsync(int healing, DamageType damageType, DamageImpact impact, Transform sourceTransform = null)
        {
            if(damageToParticles.ContainsKey(damageType)) damageToParticles[damageType].Play();
            await PoolManager.GetEffect<EffectText>().PlayHealing(transform, healing, damageType, impact, sourceTransform);
        }
    }
}