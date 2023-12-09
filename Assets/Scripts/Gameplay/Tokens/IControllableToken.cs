using Gameplay.Aggro;
using Gameplay.Interaction;

namespace Gameplay.Tokens
{
    public interface IControllableToken : IToken
    {
        public ControllableAggroManager AggroManager { get; }
    }
}