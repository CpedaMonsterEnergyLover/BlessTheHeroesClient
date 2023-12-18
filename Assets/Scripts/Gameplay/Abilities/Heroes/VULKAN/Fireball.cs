using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class Fireball : FireAbility
    {
        [Header("Fireball Fields")]
        [SerializeField] private int attackDiceAmount;
        [SerializeField] private ParticleSystem explosionParticles;



        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IUncontrollableToken creature) return;
            AnimateCast(true);

            // Throw dice
            Util.DiceUtil.CaclulateMagicDiceThrow(attackDiceAmount, Caster.MagicDiceSet, Caster.SpellPower,
                out int damage, out int energy, out int[] sides);
            await DiceManager.ThrowReplay(Caster.MagicDiceSet, attackDiceAmount, sides);
            if(Caster is HeroToken) EnergyManager.Instance.AddEnergy(Caster, energy);

            
            // Animate fireball
            var tween = transform
                .DOMove(creature.TokenTransform.position + new Vector3(0, 0.1f, 0), 10f)
                .SetSpeedBased(true)
                .SetEase(Ease.Flash);
            await tween.AsyncWaitForKill();
            AnimateCast(false);
            
            // Animate explosion
            AnimateExplosionLight();
            creature.Damage(GlobalDefinitions.FireDamageType, damage, aggroReceiver: Caster.IAggroManager);
            explosionParticles.Play();
            await UniTask.WaitUntil(() => !explosionParticles.isPlaying);
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        private void AnimateExplosionLight()
        {
            ManageTween();
            castBalllight.intensity = castBallDefaultIntensity * 1.5f;
            castBalllight.range = 3;
            lightTween = castBalllight.DOIntensity(0, 1f).OnComplete(() => lightTween = null);
        }
    }
}