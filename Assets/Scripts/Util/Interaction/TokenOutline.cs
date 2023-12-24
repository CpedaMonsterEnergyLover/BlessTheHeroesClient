using Gameplay.GameCycle;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Util.Interaction
{
    public class TokenOutline : InteractableOutline
    {
        protected override Vector3 OutlineWidth => GlobalDefinitions.TokenOutlineWidth;

        protected override void SubEvents()
        {
            base.SubEvents();
            interactable.OnInitialized += OnInitialized;
            
            if(interactable is not IToken token) return;
            token.OnActionsChanged += OnActionsOrMovementsChanged;
            token.OnMovementPointsChanged += OnActionsOrMovementsChanged;
            
            if (interactable is IControllableToken)
            {
                TurnManager.OnPlayersTurnStarted += OnTurnStarted;
                TurnManager.OnMonstersTurnStarted += OnTurnEnded;
            }
            else if (interactable is IUncontrollableToken)
            {
                TurnManager.OnMonstersTurnStarted += OnTurnStarted;
                TurnManager.OnPlayersTurnStarted += OnTurnEnded;
            }
        }

        protected override void UnsubEvents(IInteractable target)
        {
            base.UnsubEvents(target);
            target.OnInitialized -= OnInitialized;
            
            if(interactable is not IToken token) return;
            token.OnActionsChanged -= OnActionsOrMovementsChanged;
            token.OnMovementPointsChanged -= OnActionsOrMovementsChanged;
            
            if (interactable is IControllableToken)
            {
                TurnManager.OnPlayersTurnStarted -= OnTurnStarted;
                TurnManager.OnMonstersTurnStarted -= OnTurnEnded;
            }
            else if (interactable is IUncontrollableToken)
            {
                TurnManager.OnMonstersTurnStarted -= OnTurnStarted;
                TurnManager.OnPlayersTurnStarted -= OnTurnEnded;
            }
        }

        private void OnInitialized(IInteractable target)
        {
            SetEnabled(interactable is IControllableToken && interactable.CanInteract);
        }

        private void OnTurnStarted()
        {
            SetEnabled(interactable.CanInteract);
        }

        private void OnTurnEnded()
        {
            SetEnabled(false);
        }

        private void OnActionsOrMovementsChanged(IToken token) => SetEnabled(token.CanInteract);
    }
}