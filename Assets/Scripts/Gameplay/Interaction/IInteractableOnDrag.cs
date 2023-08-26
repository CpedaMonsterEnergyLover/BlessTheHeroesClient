using UI.Interaction;

namespace Gameplay.Interaction
{
    public interface IInteractableOnDrag : IInteractable
    {
        public void OnDragStart(InteractionResult result);
        public void OnDragEnd(InteractionResult target);
        public InteractionTooltipData OnDrag(InteractionResult target);
    }
}