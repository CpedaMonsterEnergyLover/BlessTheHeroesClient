using Gameplay.Interaction;
using Util.Interaction;

namespace UI.Interaction
{
    public class InteractionTooltipData
    {
        public InteractionMode Mode { get; }
        public InteractionState State { get; set; }
        public string ActionTitle { get; }
        public string ActionSubtitle { get; }

        public InteractionTooltipData(
            InteractionMode mode = InteractionMode.None, 
            InteractionState state = InteractionState.None, 
            string actionTitle = default, 
            string actionSubtitle = default)
        {
            actionTitle ??= string.Empty;
            actionSubtitle ??= string.Empty;
            Mode = mode;
            State = state;
            ActionTitle = actionTitle;
            ActionSubtitle = actionSubtitle;
        }
    }
}