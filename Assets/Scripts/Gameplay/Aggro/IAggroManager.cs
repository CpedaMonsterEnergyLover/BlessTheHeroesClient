using Gameplay.Tokens;

namespace Gameplay.Aggro
{
    public interface IAggroManager
    {
        public void AddAggro(int amount, IToken source);
        public IToken IToken { get; }
    }
}