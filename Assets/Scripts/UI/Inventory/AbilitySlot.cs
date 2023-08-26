using Gameplay.Abilities;
using Gameplay.Interaction;
using Gameplay.Inventory;
using Gameplay.Tokens;
using TMPro;
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
        [FormerlySerializedAs("text")] [SerializeField] private TMP_Text manaText;
        [SerializeField] private TMP_Text cooldownText;

        private Ability ability;

        public delegate void AbilityCastEvent(Ability ability);
        public static event AbilityCastEvent OnCastStart;
        public static event AbilityCastEvent OnCastEnd;
        public static event AbilityCastEvent OnCast;
        
        
        
        public void SetAbility(Ability target)
        {
            if(ability == target) return;
            if (ability is not null)
                ability.AbilitySlot = null;
            
            ability = target;
            if (ability is null)
            {
                manaText.transform.parent.gameObject.SetActive(false);
                icon.enabled = false;
                frameImage.enabled = false;
                icon.color = Color.white;
                cooldownText.enabled = false;
                return;
            }

            ability.AbilitySlot = this;
            SelfUpdateInteractable();
            
            if (ability is CastableAbility castable)
            {
                manaText.transform.parent.gameObject.SetActive(true);
                manaText.SetText(castable.Manacost.ToString());
                UpdateCooldownText(castable.CurrentCooldown);
            }
            else
            {
                manaText.transform.parent.gameObject.SetActive(false);
                cooldownText.enabled = false;
            }
            
            icon.sprite = ability.Icon;
            icon.enabled = true;
            if (ability is PassiveAbility)
            {
                frameImage.enabled = false;
                
                icon.transform.localScale = Vector3.one * 0.9f;
            }
            else
            {
                frameImage.enabled = true;
                icon.transform.localScale = Vector3.one;
            }
        }

        public void OnManaChanged(IToken token) => UpdateInteractable(token);

        private void SelfUpdateInteractable()
        {
            var token = TokenBrowser.Instance.SelectedToken;
            if(token is not null) 
                UpdateInteractable(token);
        }
        
        public void UpdateInteractable(IToken token)
        {
            if(ability is not CastableAbility castable) return;
            
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
            SelfUpdateInteractable();
        }

        public void SetIcon(Sprite newIcon) => icon.sprite = newIcon;


        // IDragHandler
        public void OnBeginDrag(PointerEventData eventData)
        {
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
            if(ability is null or not ActiveAbility) return;

            OnCastStart?.Invoke(ability);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(ability is null or not ActiveAbility) return;
            
            OnCastEnd?.Invoke(ability);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(ability is null or not ActiveAbility) return;

            OnCast?.Invoke(ability);
        }
        
        
        // IPointerHandler
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            InventoryManager.Instance.AbilityTooltip.SetAbility(ability);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryManager.Instance.AbilityTooltip.SetAbility(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(ability is null or not InstantAbility) return;
            if(InspectionManager.Inspecting || InteractionManager.Dragging || AbilityCaster.IsDragging) return;
            
            AbilityCaster.Cast(TokenBrowser.Instance.SelectedToken, ability);
        }
    }
}