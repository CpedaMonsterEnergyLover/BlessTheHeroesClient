using Gameplay.Tokens;

namespace Gameplay.Cards.TerrainEffects.Locations
{
    public class Swamp : TokenMoveTerrainEffect
    {
        protected override void OnTokenAdded(IToken token)
        {
            if(token is IControllableToken) token.SetActionPoints(token.ActionPoints - 1);
        }

        protected override void OnTokenRemoved(IToken token)
        {
        }
    }
}