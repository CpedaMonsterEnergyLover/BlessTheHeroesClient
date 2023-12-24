using Cysharp.Threading.Tasks;
using Gameplay.Aggro;

namespace Gameplay.Tokens
{
    public interface IUncontrollableToken : IToken
    {
        public UncontrollableAggroManager AggroManager { get; }
        public UniTask MakeTurn();
    }
}