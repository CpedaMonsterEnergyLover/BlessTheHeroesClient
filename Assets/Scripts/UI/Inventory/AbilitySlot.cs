using Gameplay.Abilities;
using Gameplay.Dice;
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
        [SerializeField] protected Image icon;
        [SerializeField] private Image frameImage;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private AbilityResourceIndicator resourceIndicator;
        
        protected Ability Ability { get; private set; }

        public delegate void ActiveAbilityCastEvent(ActiveAbility ability);
        public static event ActiveAbilityCastEvent OnCastStart;
        public static event ActiveAbilityCastEvent OnCastEnd;
        public static event ActiveAbilityCastEvent OnCast;
        
        public delegate void InstantAbilityHoverEvent(InstantAbility ability);
        public static event InstantAbilityHoverEvent OnInstantAbilityHoverEnter;
        public static event InstantAbilityHoverEvent OnInstantAbilityHoverExit;
        
        
        
        public void SetAbility(Ability target)
        {
            if (Ability == target) return;
            if (Ability is not null) Ability.AbilitySlot = null;
            Ability = target;
            resourceIndicator.SetAbility(Ability);
            target.AbilitySlot = this;
            
            UpdateInteractable(Ability.Caster);
            UpdateIcon();
            icon.enabled = true;

            if (Ability is CastableAbility castable)
                UpdateCooldownText(castable.CurrentCooldown);
            else
                cooldownText.enabled = false;


            if (Ability is PassiveAbility)
            {
                frameImage.enabled = false;
                icon.transform.localScale = Vector3.one * 0.9f;
            } else {
                frameImage.enabled = true;
                icon.transform.localScale = Vector3.one;
            }
        }

        public void ClearAbility(Sprite sprite)
        {
            if (Ability is not null) Ability.AbilitySlot = null;
            Ability = null;
            resourceIndicator.SetAbility(Ability);
            if (sprite is null) icon.enabled = false;
            else
            {
                icon.enabled = true;
                icon.sprite = sprite;
            }
            frameImage.enabled = false;
            icon.color = Color.white;
            cooldownText.enabled = false;
        }


        public void OnManaChanged(IToken token) => UpdateInteractable(token);
        
        public void UpdateInteractable(IToken token)
        {
            if(Ability is not CastableAbility castable)
            {
                icon.color = Color.white;
                return;
            }

            if (castable.Energycost > 0)
            {
                icon.color = castable.Energycost > EnergyManager.Instance.Energy ||
                             castable.CurrentCooldown > 0
                    ? Color.grey
                    : Color.white;
            }
            else
            {
                icon.color = token.TokenActionPoints == 0 || 
                             castable.Manacost > token.CurrentMana ||
                             castable.CurrentCooldown > 0
                    ? Color.grey
                    : Color.white;
            }
            
        }

        public void UpdateCooldownText(uint cooldown)
        {
            bool onCooldown = cooldown != 0;
            cooldownText.enabled = onCooldown;
            cooldownText.SetText(cooldown.ToString());
            UpdateInteractable(Ability.Caster);
        }

        public virtual void UpdateIcon() => icon.sprite = Ability.Icon;
        
        public void UpdateResourceCost() => resourceIndicator.SetAbility(Ability);


        // IDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
            if(Ability is not ActiveAbility active) return;
            OnCastStart?.Invoke(active);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(Ability is not ActiveAbility active) return;
            OnCastEnd?.Invoke(active);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(Ability is not ActiveAbility active) return;
            OnCast?.Invoke(active);
        }
        
        protected virtual void UpdateTooltipOnPointerEnter() => InventoryManager.Instance.AbilityTooltip.SetAbility(Ability);
        protected virtual void UpdateTooltipOnPointerExit() => InventoryManager.Instance.AbilityTooltip.SetAbility(null);
        protected virtual void UpdateTooltipOnPointerClick() => InventoryManager.Instance.AbilityTooltip.SetAbility(null);
        
        
        // IPointerHandler
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            UpdateTooltipOnPointerEnter();
            if(Ability is InstantAbility instant) OnInstantAbilityHoverEnter?.Invoke(instant);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            UpdateTooltipOnPointerExit();
            if(Ability is InstantAbility instant) OnInstantAbilityHoverExit?.Invoke(instant);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if(Ability is not InstantAbility instant) return;
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;

            if (eventData.clickCount == 1)
            {
                UpdateTooltipOnPointerClick();
                AbilityCaster.Cast(TokenBrowser.Instance.SelectedToken, Ability.Caster.TokenCard, instant);  
            }
        }
    }
}