using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using Scriptable;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class Nuke : FireAbility
    {
        [Header("NukeFields")]
        [SerializeField] private DamageType damageType;
        [SerializeField] private int damage;
        [SerializeField] private ParticleSystem nukeParticles;



        public override bool ValidateTarget(IInteractable target) => ValidateOpenedCard(target);
        public override async UniTask Cast(IInteractable target)
        {
            if (target is not Card {IsOpened: true} card) return;
            AnimateCast(true);
            
            // Animate fireball
            var tween = transform.DOJump(
                    card.transform.position + new Vector3(0, 0.1f, 0), 
                    1f,
                    1,
                    1f)
                .SetEase(Ease.OutFlash);
            await tween.AsyncWaitForKill();
            AnimateCast(false);
            
            // Animate explosion
            ManageTween();
            nukeParticles.Play();
            foreach (IUncontrollableToken creature in card.Creatures) 
                creature.Damage(GlobalDefinitions.FireDamageType, damage, aggroReceiver: Caster.IAggroManager);
            castBalllight.intensity = 15f;
            castBalllight.range = 3f;
            castBalllight.DOIntensity(0, 1f);
            await UniTask.WaitUntil(() => !nukeParticles.isPlaying);
            transform.localPosition = new Vector3(0, 0.5f, 0);
            castBalllight.range = 1f;
        }
    }
}