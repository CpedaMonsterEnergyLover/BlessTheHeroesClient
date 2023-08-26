using Gameplay.Tokens;

namespace Util.Interface
{
    public interface IHasTokenDragEvent
    {
        public void OnTokenStartDraggingEventInvoked(IToken token);

        public void OnTokenEndDraggingEventInvoked(IToken token);
    }
}