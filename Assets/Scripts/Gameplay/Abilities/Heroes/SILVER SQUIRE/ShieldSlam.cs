using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class ShieldSlam : ActiveAbility, IEffectApplier
    {
        [SerializeField] private int damage;
        [SerializeField] private ShieldSlamBuffEffect shieldSlamBuffEffect;
        [SerializeField] private int buffDuration;
        
        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not Card card) return;

            await Caster.Move(card);
            var enemies = Caster.TokenCard.Creatures.Where(c => !c.Dead);
            foreach (IUncontrollableToken token in enemies)
            {
                token.Damage(damage, aggroManager: Caster.IAggroManager);
                Caster.IAggroManager.AddAggro(damage, null);
            }
            Caster.BuffManager.ApplyEffect(this, shieldSlamBuffEffect, buffDuration);
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is Card { IsOpened: true } card && card.HasSpaceForHero();
        }
    }
}