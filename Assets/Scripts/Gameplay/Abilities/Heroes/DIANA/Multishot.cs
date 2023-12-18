using System.Linq;
using Cysharp.Threading.Tasks;
using Effects;
using Gameplay.Cards;
using Gameplay.Tokens;
using UnityEngine;
using Util;
using Util.Animators;
using Util.Enums;

namespace Gameplay.Abilities
{
    public class Multishot : PassiveAbility
    {
        [SerializeField] private Sprite secondIcon;
        [SerializeField] private Sprite thirdIcon;
        [SerializeField] private int arrowDamage = 1;

        private readonly Sprite[] spriteToStage = new Sprite[3];
        private int stage;

        public override Sprite Icon => spriteToStage[stage];

        
        
        // Unity methods
        private void Awake()
        {
            spriteToStage[0] = icon;
            spriteToStage[1] = secondIcon;
            spriteToStage[2] = thirdIcon;
        }
        
        
        // Class methods
        protected override void OnTokenSet(IToken token)
        {
            Caster.OnBeforeAttackPerformed += BeforeAttackPerformed;
        }

        private void OnDisable() => Caster.OnBeforeAttackPerformed -= BeforeAttackPerformed;


        private void BeforeAttackPerformed(IToken token, IToken attackTarget, AttackType attackType, int damage, int _)
        {
            if(token is not HeroToken ||
               attackTarget is not CreatureToken creatureToken ||
               attackType is not AttackType.Ranged) return;

            Card card = attackTarget.TokenCard;
            var creatures = card.Creatures;
            creatures.Remove(creatureToken);
            var targets = creatures.OrderBy(_ => Random.value).Take(stage).ToArray();
            if(targets.Length == 0)
            {
                UpgradeStage();
                return;
            }

            RangedAttackAnimator attackAnimator = token.AttackAnimatorManager.Ranged;
            int targetsAmount = targets.Length;
            for (int i = 0; i < targetsAmount; i++)
            {
                var target = targets[i];
                var arrow = PoolManager.GetEffect<EffectArrow>();
                arrow.SetPosition(attackAnimator.ArrowPosition);
                arrow.SetRotation(attackAnimator.GetRotation(target.TokenTransform.position));
                arrow.Shoot(target.TokenTransform).Forget();
                target.Damage(GlobalDefinitions.PhysicalDamageType, arrowDamage, aggroReceiver: Caster.IAggroManager).Forget();
            }
            
            UpgradeStage();
        }

        private void UpgradeStage()
        {
            stage++;
            if (stage == 3) stage = 0;
            
            if(AbilitySlot is not null) AbilitySlot.UpdateIcon();
        }
    }
}