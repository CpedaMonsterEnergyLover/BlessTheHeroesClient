using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class Fireball : FireAbility
    {
        [Header("Fireball Fields")]
        [SerializeField] private int attackDiceAmount;
        [SerializeField] private ParticleSystem explosionParticles;
        

        
        public override bool ValidateTarget(IInteractable target)
        {
            return target is CreatureToken;
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not CreatureToken creature) return;
            AnimateCast(true);

            // Throw dice
            Util.DiceUtil.CaclulateMagicDiceThrow(attackDiceAmount, DiceManager.MagicDiceSet, out int damage, out int[] sides);
            await DiceManager.ThrowReplay(DiceManager.MagicDiceSet, attackDiceAmount, sides);
            
            // Animate fireball
            var tween = transform
                .DOMove(creature.transform.position + new Vector3(0, 0.1f, 0), 10f)
                .SetSpeedBased(true)
                .SetEase(Ease.Flash);
            await tween.AsyncWaitForKill();
            AnimateCast(false);
            
            // Animate explosion
            AnimateExplosionLight();
            creature.Damage(damage);
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