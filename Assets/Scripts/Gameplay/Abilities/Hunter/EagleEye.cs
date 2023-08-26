using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Interaction;

namespace Gameplay.Abilities.Hunter
{
    public class EagleEye : ActiveAbility
    {
        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is Card { IsOpened: false };
        }

        public override UniTask Cast(IInteractable target)
        {
            if (target is Card {IsOpened: false} card) 
                card.Open();
            return default;
        }
    }
}