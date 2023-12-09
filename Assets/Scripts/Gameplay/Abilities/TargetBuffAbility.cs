﻿using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class TargetBuffAbility : ActiveAbility, IEffectApplier
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int effectDuration;
        [SerializeField] private ParticleSystem applyParticles;
        [SerializeField] private bool friendly;

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return;
            token.BuffManager.ApplyEffect(this, effectToApply, effectDuration);
            
            if (applyParticles is not null)
            {
                applyParticles.transform.position = token.TokenTransform.position;
                applyParticles.Play();
                await UniTask.WaitUntil(() => !applyParticles.isPlaying);
            }
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return friendly
                ? ValidateAlly(target)
                : ValidateEnemy(target);
        }
    }
}