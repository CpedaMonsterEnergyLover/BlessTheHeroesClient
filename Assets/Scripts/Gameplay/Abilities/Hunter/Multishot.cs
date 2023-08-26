using System.Linq;
using Cysharp.Threading.Tasks;
using Effects;
using Gameplay.GameField;
using Gameplay.Tokens;
using UI.Elements;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities.Hunter
{
    public class Multishot : PassiveAbility
    {
        [SerializeField] private Sprite secondIcon;
        [SerializeField] private Sprite thirdIcon;

        private readonly Sprite[] spriteToStage = new Sprite[3];
        private int stage;

        public override Sprite Icon => spriteToStage[stage];

        
        
        // Unity methods
        private void Awake()
        {
            spriteToStage[0] = secondIcon;
            spriteToStage[1] = thirdIcon;
            spriteToStage[2] = icon;
        }
        
        
        // Class methods
        protected override void OnTokenSet(IToken token)
        {
            base.OnTokenSet(token);
            Token.OnAttackPerformed += OnAttackPerformed;
        }

        private void OnDisable() => Token.OnAttackPerformed -= OnAttackPerformed;


        private void OnAttackPerformed(IToken token, IToken attackTarget, AttackType attackType, int damage, int _)
        {
            if(token is not HeroToken ||
               attackType is not AttackType.Ranged) return;

            Card card = attackTarget.TokenCard;
            var targets = card.Creatures.OrderBy(_ => Random.value).Take(stage).ToArray();
            if(targets.Length == 0)
            {
                UpgradeStage();
                return;
            }

            RangedAttackVisualizer attackVisualizer = token.RangedAttackVisualizer;
            for (int i = 0; i < targets.Length; i++)
            {
                int arrowDamage = Mathf.Clamp(damage - (i + 1), 0, int.MaxValue);
                if(arrowDamage == 0) continue;

                var target = targets[i];
                var arrow = EffectsManager.GetEffect<EffectArrow>();
                arrow.SetPosition(attackVisualizer.ArrowPosition);
                arrow.SetRotation(attackVisualizer.GetRotation(target.TokenTransform.position));
                arrow.Shoot(target.TokenTransform).Forget();
                target.Damage(arrowDamage);
            }
            UpgradeStage();
        }

        private void UpgradeStage()
        {
            stage++;
            if (stage == 3) stage = 0;
            
            UpdateSlotIcon();
        }

        private void UpdateSlotIcon()
        {
            if(AbilitySlot is null) return;
            AbilitySlot.SetIcon(Icon);
        }
    }
}