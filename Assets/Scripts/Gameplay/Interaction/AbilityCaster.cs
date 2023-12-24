using System.Collections.Generic;
using System.Linq;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.Tokens;
using UI;
using UnityEngine;
using Util;
using Util.Interaction;

namespace Gameplay.Interaction
{
    public class AbilityCaster : MonoBehaviour
    {
        private IControllableToken caster;
        private static readonly List<IInteractable> TargetsCache = new();
        public static bool IsDragging { get; private set; }

        public delegate void OutlineEvent();
        public static event OutlineEvent OnAbilityCastStart;
        public static event OutlineEvent OnAbilityCastEnd;
        
        

        private void OnEnable()
        {
            AbilitySlot.OnInstantAbilityHoverEnter += OnInstantAbilityHoverEnter;
            AbilitySlot.OnInstantAbilityHoverExit += OnInstantAbilityHoverExit;
            AbilitySlot.OnCastStart += OnCastStart;
            AbilitySlot.OnCastEnd += OnCastEnd;
            AbilitySlot.OnCast += OnCast;
        }

        private void OnDisable()
        {
            AbilitySlot.OnInstantAbilityHoverEnter -= OnInstantAbilityHoverEnter;
            AbilitySlot.OnInstantAbilityHoverExit -= OnInstantAbilityHoverExit;
            AbilitySlot.OnCastStart -= OnCastStart;
            AbilitySlot.OnCastEnd -= OnCastEnd;
            AbilitySlot.OnCast -= OnCast;
        }


        private void OnCastStart(ActiveAbility ability)
        {
            if(IsDragging ||
               ability.Caster is not IControllableToken controllable ||
               !ability.ApproveCast(controllable)) return;

            caster = controllable;
            TargetsCache.Clear();
            ability.GetTargetsList(TargetsCache);
            UpdateOutlinesOnCastStart();
            
            IsDragging = true;
            ability.OnCastStart();
            var interactionResult = InteractionManager.GetInteractionResult();
            caster.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
        }

        private void OnCast(ActiveAbility ability)
        {
            if(!IsDragging) return;

            IsDragging = true;
            var interactionResult = InteractionManager.GetInteractionResult();
            bool validTarget = TargetsCache.Contains(interactionResult.Target);
            caster.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
            InteractionState state = interactionResult.IsValid
                ? validTarget
                    ? InteractionState.Allow
                    : InteractionState.Abandon
                : InteractionState.None;
            caster.InteractionLine.SetInteractableColor(state);
            caster.InteractionLine.UpdatePosition(interactionResult.IntersectionPoint);
        }

        private void OnCastEnd(ActiveAbility ability)
        {
            if (!IsDragging) return;
            
            UpdateOutlinesOnCastEnd();
            IsDragging = false;
            ability.OnCastEnd();
            var interactionResult = InteractionManager.GetInteractionResult();
            caster.InteractionLine.Disable();
            Cast(caster, interactionResult.Target, ability);
            caster = null;
        }

        private void OnInstantAbilityHoverEnter(InstantAbility instant)
        {
            TargetsCache.Clear();
            instant.GetTargetsList(TargetsCache);
            UpdateOutlinesOnCastStart();
        }

        private void OnInstantAbilityHoverExit(InstantAbility instant)
        {
            UpdateOutlinesOnCastEnd();
        }

        public static void Cast(IToken caster, IInteractable target, CastableAbility castable)
        {
            if (!castable.ApproveCast(caster) || 
                !TargetsCache.Contains(target)) return;

            if (castable.Energycost > 0)
            {
                if (!EnergyManager.Instance.TryDrainEnergy(castable.Energycost)) return;
            } 
            else if (castable.Manacost > 0 && !caster.DrainMana(castable.Manacost)) return;


            if (castable.Healthcost > 0)
                caster.Damage(GlobalDefinitions.BloodDamageType, castable.Healthcost, null);
            
            castable.SetOnCooldown();
            castable.Cast(target);
            if (castable.Energycost == 0 && castable.RequiresAct) 
                caster.SetActionPoints(caster.ActionPoints - 1);
        }

        private static void UpdateOutlinesOnCastStart()
        {
            OnAbilityCastStart?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.EnableOutline();
        }
        
        private static void UpdateOutlinesOnCastEnd()
        {
            OnAbilityCastEnd?.Invoke();
            foreach (IInteractable t in TargetsCache.Where(cached => !cached.Dead)) t.UpdateOutline();
        }
    }
}