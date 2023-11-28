using Gameplay.Abilities;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Tokens;
using TMPro;
using UI.Elements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class AbilitySlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image frameImage;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private AbilityResourceIndicator resourceIndicator;
        
        private Ability ability;

        public delegate void AbilityCastEvent(ActiveAbility ability);
        public static event AbilityCastEvent OnCastStart;
        public static event AbilityCastEvent OnCastEnd;
        public static event AbilityCastEvent OnCast;
        public delegate void InstantAbilityHoverEvent(InstantAbility ability);
        public static event InstantAbilityHoverEvent OnInstantAbilityHoverEnter;
        public static event InstantAbilityHoverEvent OnInstantAbilityHoverExit;
        
        
        public void SetAbility(Ability target)
        {
            if (ability == target) return;
            if (ability is not null)
                ability.AbilitySlot = null;
            
            ability = target;
            resourceIndicator.SetAbility(ability);
            if (ability is null)
            {
                icon.enabled = false;
                frameImage.enabled = false;
                icon.color = Color.white;
                cooldownText.enabled = false;
                return;
            }

            ability.AbilitySlot = this;
            
            UpdateInteractable(ability.Caster);
            icon.sprite = ability.Icon;
            icon.enabled = true;

            if (ability is CastableAbility castable)
                UpdateCooldownText(castable.CurrentCooldown);
            else
                cooldownText.enabled = false;


            if (ability is PassiveAbility)
            {
                frameImage.enabled = false;
                icon.transform.localScale = Vector3.one * 0.9f;
            } else {
                frameImage.enabled = true;
                icon.transform.localScale = Vector3.one;
            }
        }

        public void OnManaChanged(IToken token) => UpdateInteractable(token);
        
        public void UpdateInteractable(IToken token)
        {
            if(ability is not CastableAbility castable)
            {
                icon.color = Color.white;
                return;
            }
            
            icon.color = token.TokenActionPoints == 0 || 
                         castable.Manacost > token.CurrentMana ||
                         castable.CurrentCooldown > 0
                ? Color.grey
                : Color.white;
        }

        public void UpdateCooldownText(uint cooldown)
        {
            bool onCooldown = cooldown != 0;
            cooldownText.enabled = onCooldown;
            cooldownText.SetText(cooldown.ToString());
            UpdateInteractable(ability.Caster);
        }

        public void UpdateIcon() => icon.sprite = ability.Icon;
        
        public void UpdateResourceCost() => resourceIndicator.SetAbility(ability);


        // IDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
            if(ability is not ActiveAbility active) return;
            OnCastStart?.Invoke(active);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(ability is not ActiveAbility active) return;
            OnCastEnd?.Invoke(active);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(ability is not ActiveAbility active) return;
            OnCast?.Invoke(active);
        }
        
        
        // IPointerHandler
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            InventoryManager.Instance.AbilityTooltip.SetAbility(ability);
            if(ability is InstantAbility instant) OnInstantAbilityHoverEnter?.Invoke(instant);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
            if(ability is InstantAbility instant) OnInstantAbilityHoverExit?.Invoke(instant);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(ability is not InstantAbility instant) return;
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
            AbilityCaster.Cast(TokenBrowser.Instance.SelectedToken, ability.Caster.TokenCard, instant);
        }
    }
}