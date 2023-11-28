using Gameplay.BuffEffects;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class BattleCryBuffEffect : StackableStatBuffEffect
    {
        [SerializeField] private int movementPoints;
        
        protected override void OnApplied()
        {
            base.OnApplied();
            IToken token = Manager.Token;
            token.SetMovementPoints(token.MovementPoints + movementPoints);
        }

        protected override void OnRemoved()
        {
            IToken token = Manager.Token;
            token.SetMovementPoints(token.MovementPoints - movementPoints);
            base.OnRemoved();
        }
    }
}