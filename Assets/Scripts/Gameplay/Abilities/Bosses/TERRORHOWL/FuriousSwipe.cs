using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Interaction;
using Gameplay.Tokens;
using System.Linq;
using Gameplay.Cards;
using Util;

namespace Gameplay.Abilities
{
    public class FuriousSwipe : AutoAbility
    {
        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not Card card) return;
            
            var animationTween = DOTween.Sequence()
                .Append(Caster.TokenTransform.DOLocalMoveY(0.5f, 0.75f))
                .Append(Caster.TokenTransform.DOLocalMoveY(0, 0.75f));
            await animationTween.AsyncWaitForKill();
            
            foreach (IControllableToken hero in card.Heroes)
            {
                hero.Damage(GlobalDefinitions.PhysicalDamageType,3, Caster);
            }
        }

        public override bool GetTarget(out IInteractable target)
        {
            target = Caster.Card;
            return Caster.Card.Heroes.Count(h => !h.Dead) > 0;
        }
    }
}