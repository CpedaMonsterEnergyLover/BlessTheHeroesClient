using UnityEngine;

namespace Gameplay.Interaction
{
    public class InteractionResult
    {
        public IInteractable Target { get; }
        public Vector3 IntersectionPoint { get; }
        public bool IsValid { get; }

        public InteractionResult(bool isValid, IInteractable target = null, Vector3 intersectionPoint = default)
        {
            Target = target;
            IntersectionPoint = intersectionPoint;
            IsValid = isValid;
        }
    }
}