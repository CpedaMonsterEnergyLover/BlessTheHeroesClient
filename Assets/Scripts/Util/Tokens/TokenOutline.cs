using Gameplay.Tokens;
using UnityEngine;
using Util.Interaction;

namespace Util.Tokens
{
    public abstract class TokenOutline<T> : InteractableOutline
    where T : IToken
    {
        private T target;

        protected override void Awake()
        {
            base.Awake();
            if (Interactable is not T casted)
            {
                Debug.LogError($"Interactable on GO {gameObject.name} is not a type of {typeof(T)}, which is required by TokenOutline.");
                return;
            }

            target = casted;
            target.OnDestroyed += OnTargetDestroy;
            SubEvents(casted);
        }

        private void OnTargetDestroy(IToken token) => UnsubEvents((T) token);
        
        protected abstract void SubEvents(T token);
        protected abstract void UnsubEvents(T token);
    }
}