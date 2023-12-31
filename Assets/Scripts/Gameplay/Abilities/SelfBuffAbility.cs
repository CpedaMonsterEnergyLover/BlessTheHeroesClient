﻿using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class SelfBuffAbility : InstantAbility, IEffectApplier
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int effectDuration;
        
        public override bool ValidateTarget(IInteractable target) => target is Card || target.Equals(Caster);

        public override async UniTask Cast(IInteractable target)
        {
            Caster.BuffManager.ApplyEffect(this, effectToApply, effectDuration);
        }

        public IToken Token => Caster;
    }
}