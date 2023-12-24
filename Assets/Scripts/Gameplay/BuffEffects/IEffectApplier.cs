using Gameplay.Tokens;

namespace Gameplay.BuffEffects
{
    public interface IEffectApplier
    {
        public IToken Token { get; }
    }
}