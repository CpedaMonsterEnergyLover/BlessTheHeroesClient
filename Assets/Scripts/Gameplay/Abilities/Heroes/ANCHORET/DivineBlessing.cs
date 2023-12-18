using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class DivineBlessing : InstantAbility, IEffectApplier
    {
        [SerializeField] private DivineBlessingBuffEffect divineBlessingBuffEffect;
        [SerializeField] private int buffDuration;
        [SerializeField] private ParticleSystem particles;


        public override bool ValidateTarget(IInteractable target) => target is Card || ValidateAlly(target);

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not Card card) return;
            
            foreach (IControllableToken controllable in card.Heroes)
            {
                controllable.BuffManager.ApplyEffect(this, divineBlessingBuffEffect, buffDuration);
            }
            
            particles.Play();
            await UniTask.WaitUntil(() => !particles.isPlaying);
        }
    }
}