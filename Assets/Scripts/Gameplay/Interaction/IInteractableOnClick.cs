namespace Gameplay.Interaction
{
    public interface IInteractableOnClick : IInteractable
    {
        public bool CanClick { get; }
        public void OnClick(InteractionResult result, int clickCount);
    }
}