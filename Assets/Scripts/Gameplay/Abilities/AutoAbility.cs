using Gameplay.Interaction;
using UnityEngine;

namespace Gameplay.Abilities
{
    public abstract class AutoAbility : InstantAbility
    {
        [SerializeField] private int priority;

        public int Priority => priority;
        
        public abstract bool GetTarget(out IInteractable target);
    }
}