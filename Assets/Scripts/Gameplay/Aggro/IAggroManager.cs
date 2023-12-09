using Gameplay.Tokens;

namespace Gameplay.Aggro
{
    public interface IAggroManager
    {
        public void Activate(IToken wearer);
        public void AddAggro(int amount, IToken source, bool mirrored = false);
        public void RemoveAggro(int amount, IToken source, bool mirrored = false);
        public IToken Wearer { get; }
    }
}