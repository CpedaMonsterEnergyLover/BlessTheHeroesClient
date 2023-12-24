using DG.Tweening;
using Gameplay.Tokens;
using TMPro;
using UI.Browsers;
using UnityEngine;

namespace UI.Tooltips
{
    public class InventoryTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform pivot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private AbilityTooltip abilityTooltip;
        
        

        public void SetItem(Scriptable.Item item, int amount)
        {
            if(item is null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            bool isEquipment = false;
            if (item is Scriptable.Equipment equipment)
            {
                isEquipment = true;
                abilityTooltip.SetAbility(equipment.HasAbility ? equipment.Ability : null, false);

                statsText.SetText(equipment.GetStatsStringBuilder());
                if (TokenBrowser.SelectedToken is HeroToken heroToken)
                {
                    bool trinket = equipment is Scriptable.Trinket;
                    actionText.SetText(equipment is Scriptable.Weapon weapon && heroToken.Scriptable.AttackType == weapon.AttackType ||
                                       equipment is Scriptable.Armor armor && heroToken.Scriptable.ArmorType == armor.ArmorType ||
                                       trinket
                        ? heroToken.ActionPoints > 0 
                            ? trinket 
                                ? "<color=green>Double click to equip \n(shift - equip in second slot)" 
                                : "<color=green>Double click to equip \n(costs 1 ACT)" 
                            : "<color=red>Not enough ACT to equip"
                        : "<color=red>Selected hero cannot equip this" );
                    actionText.gameObject.SetActive(true);
                } else actionText.gameObject.SetActive(false);
            } else if (item is Scriptable.Usable && 
                       TokenBrowser.SelectedToken is HeroToken heroToken)
            {
                actionText.SetText(heroToken.ActionPoints > 0 
                    ? "<color=green>Double click to use \n(costs 1 ACT)" 
                    : "<color=red>Not enough ACT to use");
                actionText.gameObject.SetActive(true);
            }
            else actionText.gameObject.SetActive(false);

            if(!isEquipment)
                abilityTooltip.gameObject.SetActive(false);
            statsText.gameObject.SetActive(isEquipment);
            
            int price = item.Price;
            titleText.SetText(item.Name);
            categoryText.SetText(item.CategoryName);
            priceText.SetText(amount == 1 ? $"Sell price: {price}g" : $"Sell price: {price}g ({price * amount}g)");
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