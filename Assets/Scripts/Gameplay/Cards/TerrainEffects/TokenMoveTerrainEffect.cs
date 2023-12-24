using Gameplay.Tokens;

namespace Gameplay.Cards.TerrainEffects
{
    public abstract class TokenMoveTerrainEffect : TerrainEffect
    {
        protected override void OnApplied()
        {
            Manager.Card.OnTokenAdded += OnTokenAdded;
            Manager.Card.OnTokenRemoved += OnTokenRemoved;
        }

        protected override void OnRemoved()
        {
            Manager.Card.OnTokenAdded -= OnTokenAdded;
            Manager.Card.OnTokenRemoved -= OnTokenRemoved;
        }

        private void OnDestroy() => OnRemoved();

        protected abstract void OnTokenAdded(IToken token);
        protected abstract void OnTokenRemoved(IToken token);
        protected override void OnTick() { }
    }
}