﻿using System.Collections.Generic;
using Gameplay.Abilities;
using Gameplay.Dice;
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

        private IControllableToken caster;
        private static InteractionManager InteractionManager { get; set; }
        private static readonly List<IInteractable> TargetsCache = new();
        public static bool IsDragging { get; private set; }


        
        private void Awake()
        {
            InteractionManager = interactionManager;
            interactionManager = null;
        }

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
            if(ability.Caster is not IControllableToken controllable ||
               !ability.ApproveCast(controllable)) return;

            caster = controllable;
            caster.InvokeStartDraggingEvent();
            TargetsCache.Clear();
            ability.GetTargetsList(TargetsCache);
            foreach (IInteractable target in TargetsCache) 
                target.Outline.SetEnabled(true);
            IsDragging = true;
            ability.OnCastStart();
            var interactionResult = InteractionManager.GetInteractionResult();
            caster.InteractionLine.SetEnabled(interactionResult.IsValid, interactionResult.IntersectionPoint);
        }

        private void OnCast(ActiveAbility ability)
        {
            if(!IsDragging) return;
            
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

            caster.InvokeEndDraggingEvent();
            foreach (IInteractable target in TargetsCache) 
                target.UpdateOutlineByCanInteract();
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
            instant.Caster.InvokeStartDraggingEvent();
            foreach (IInteractable target in TargetsCache) 
                target.Outline.SetEnabled(true);
        }

        private void OnInstantAbilityHoverExit(InstantAbility instant)
        {
            instant.Caster.InvokeEndDraggingEvent();
            foreach (IInteractable target in TargetsCache) 
                target.UpdateOutlineByCanInteract();
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
                caster.Damage(castable.Healthcost);
            
            castable.SetOnCooldown();
            castable.Cast(target);
            if (castable.Energycost == 0 && castable.RequiresAct) 
                caster.SetActionPoints(caster.ActionPoints - 1);
        }
    }
}