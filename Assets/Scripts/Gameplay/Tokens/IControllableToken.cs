using Gameplay.Aggro;

namespace Gameplay.Tokens
{
    public interface IControllableToken : IToken
    {
        public ControllableAggroManager AggroManager { get; }
    }
}