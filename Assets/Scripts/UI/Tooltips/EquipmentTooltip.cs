using DG.Tweening;
using Gameplay.Abilities;
using TMPro;
using UnityEngine;

namespace UI.Tooltips
{
    public class EquipmentTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform pivot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private AbilityTooltip abilityTooltip;

        
        
        public void SetItem(Scriptable.Equipment item, Ability ability)
        {
            if(item is null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (item.HasAbility && ability is not null)
                abilityTooltip.SetAbility(ability, false);
            else 
                abilityTooltip.gameObject.SetActive(false);
            
            int price = item.Price;
            statsText.SetText(item.GetStatsStringBuilder());
            titleText.SetText(item.Name);
            categoryText.SetText(item.CategoryName);
            priceText.SetText($"Sell Price: {price}g");
            descriptionText.SetText(item.Description.Equals(string.Empty) ? "No description" : item.Description);
            gameObject.SetActive(true);
            PlayAnimation();
        }
        
        private void PlayAnimation()
        {
            pivot.localScale = Vector3.zero;
            pivot.DOScale(Vector3.one, 0.15f);
        }
    }
}