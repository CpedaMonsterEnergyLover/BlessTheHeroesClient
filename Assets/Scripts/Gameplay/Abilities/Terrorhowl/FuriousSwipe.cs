using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;

namespace Gameplay.Abilities.Terrorhowl
{
    public class FuriousSwipe : AutoAbility
    {
        protected override void OnTokenSet(IToken token)
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not Card card) return;
            
            var animationTween = DOTween.Sequence()
                .Append(Caster.TokenTransform.DOLocalMoveY(0.5f, 0.75f))
                .Append(Caster.TokenTransform.DOLocalMoveY(0, 0.75f));
            await animationTween.AsyncWaitForKill();
            
            foreach (IControllableToken hero in card.Heroes)
            {
                hero.Damage(3);
            }
        }

        public override bool GetTarget(out IInteractable target)
        {
            target = Caster.TokenCard;
            return Caster.TokenCard.HeroesAmount > 0;
        }
    }
}