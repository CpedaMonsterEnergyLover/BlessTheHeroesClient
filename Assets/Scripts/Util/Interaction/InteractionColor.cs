using System;
using UnityEngine;

namespace Util.Interaction
{
    public static class InteractionColor
    {
        public static Color Get(InteractionState state)
        {
            return state switch
            {
                InteractionState.None => Color.white,
                InteractionState.Allow => Color.green,
                InteractionState.Abandon => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}