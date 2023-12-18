using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

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
                token.Damage(GlobalDefinitions.PhysicalDamageType, damage, aggroReceiver: Caster.IAggroManager);
                token.AggroManager.AddAggro(damage, Caster);
            }
            Caster.BuffManager.ApplyEffect(this, shieldSlamBuffEffect, buffDuration);
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is Card { IsOpened: true } card && card.HasSpaceForHero();
        }
    }
}