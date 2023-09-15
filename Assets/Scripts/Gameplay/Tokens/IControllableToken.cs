using Gameplay.Interaction;

namespace Gameplay.Tokens
{
    public interface IControllableToken : IToken
    {
        public InteractionLine InteractionLine { get; }
        public bool CanInteract { get; }
    }
}