using Cysharp.Threading.Tasks;

namespace Gameplay.Tokens
{
    public interface IUncontrollableToken : IToken
    {
        public bool CanBeTargeted { get; }
        public UniTask MakeTurn();
    }
}