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

        private IToken token;
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
            if(ability is not ActiveAbility active ||
                TokenBrowser.Instance.SelectedToken is not HeroToken hero || 
               !active.ApproveCast(hero)) return;
            
            IsDragging = true;
            token = TokenBrowser.Instance.SelectedToken;
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
            if(valid) Cast(token, ability);
            token = null;
        }

        public static void Cast(IToken token, Ability ability)
        {
            if (ability is not CastableAbility castable ||
                token is not HeroToken hero || 
                !castable.ApproveCast(hero) ||
                !((IHasMana) hero).DrainMana(castable.Manacost)) return;
            
            castable.SetOnCooldown();
            castable.Cast(InteractionManager.GetInteractionResult().Target);
            hero.SetActionPoints(hero.ActionPoints - 1);
        }
    }
}