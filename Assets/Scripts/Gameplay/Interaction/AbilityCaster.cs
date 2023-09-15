using Gameplay.Abilities;
using Gameplay.Tokens;
using UI;
using UnityEngine;
using Util.Interaction;
using Util.Tokens;

namespace Gameplay.Interaction
{
    public class AbilityCaster : MonoBehaviour
    {
        [SerializeField] private InteractionManager interactionManager;

        private IControllableToken token;
        private static InteractionManager InteractionManager { get; set; }
        
        public static bool IsDragging { get; private set; }


        private void Awake()
        {
            InteractionManager = interactionManager;
            interactionManager = null;
        }

        private void OnEnable()
        {
            AbilitySlot.OnCastStart += OnCastStart;
            AbilitySlot.OnCastEnd += OnCastEnd;
            AbilitySlot.OnCast += OnCast;
        }

        private void OnDisable()
        {
            AbilitySlot.OnCastStart -= OnCastStart;
            AbilitySlot.OnCastEnd -= OnCastEnd;
            AbilitySlot.OnCast -= OnCast;
        }



        private void OnCastStart(Ability ability)
        {
            var selectedToken = TokenBrowser.Instance.SelectedToken;
            if(ability is not ActiveAbility active ||
               selectedToken is not IControllableToken controllable ||
               !active.ApproveCast(selectedToken)) return;
            
            IsDragging = true;
            token = controllable;
            active.OnCastStart();
            var interactionResult = InteractionManager.GetInteractionResult();
            token.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
        }

        private void OnCast(Ability ability)
        {
            if(!IsDragging || ability is not ActiveAbility active) return;
            
            var interactionResult = InteractionManager.GetInteractionResult();
            bool validTarget = active.ValidateTarget(interactionResult.Target);
            token.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
            InteractionState state = interactionResult.IsValid
                ? validTarget
                    ? InteractionState.Allow
                    : InteractionState.Abandon
                : InteractionState.None;
            token.InteractionLine.SetInteractableColor(state);
            token.InteractionLine.UpdatePosition(interactionResult.IntersectionPoint);
        }

        private void OnCastEnd(Ability ability)
        {
            if(!IsDragging || ability is not ActiveAbility active) return;

            IsDragging = false;
            active.OnCastEnd();
            var interactionResult = InteractionManager.GetInteractionResult();
            token.InteractionLine.SetEnabled(false, interactionResult.IntersectionPoint);
            bool valid = active.ValidateTarget(interactionResult.Target);
            if(valid) Cast(token, active);
            token = null;
        }

        public static void Cast(IToken caster, CastableAbility castable)
        {
            if (!castable.ApproveCast(caster)) return;

            var target = castable is InstantAbility 
                ? caster.TokenCard
                : InteractionManager.GetInteractionResult().Target;
            
            if(castable is ActiveAbility active && !active.ValidateTarget(target)) return;
            
            if(castable.Manacost > 0 && !((IHasMana) caster).DrainMana(castable.Manacost)) return;
            
            castable.SetOnCooldown();
            castable.Cast(target);
            if(castable.RequiresAct) caster.SetActionPoints(caster.ActionPoints - 1);
        }
    }
}