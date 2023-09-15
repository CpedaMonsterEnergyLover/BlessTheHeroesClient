using Gameplay.Interaction;
using Gameplay.Tokens;

namespace Gameplay.Abilities
{
    public abstract class ActiveAbility : CastableAbility
    {
        public abstract void OnCastStart();
        public abstract void OnCastEnd();
        public abstract bool ValidateTarget(IInteractable target);
    }
}