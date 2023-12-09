﻿using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class TargetDamageAbility : ActiveAbility
    {
        [SerializeField] private int damage;
        
        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return;
            
            await token.Damage(damage, aggroManager: Caster.IAggroManager);
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }
    }
}