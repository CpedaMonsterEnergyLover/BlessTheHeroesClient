using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Abilities
{
    public class Duel : ActiveAbility, IEffectApplier
    {
        [SerializeField] private DuelBuffEffect duelBuffEffect;
        [SerializeField] private int buffDuration;
        [SerializeField] private int numberOfAttacks;

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IUncontrollableToken creature) return;

            bool won = false;
            int attackCounter = 0;
            while (attackCounter < numberOfAttacks)
            {
                await Caster.Attack(creature);
                if (creature.Dead)
                {
                    won = true;
                    break;
                }
                
                await creature.Attack(Caster);
                if(Caster.Dead) break;
                
                attackCounter++;
            }
            
            if(won) Caster.BuffManager.ApplyEffect(this, duelBuffEffect, buffDuration);
        }

        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);
    }
}