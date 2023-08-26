using DG.Tweening;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltips
{
    public class InventoryTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text categoryText;
        [SerializeField] private TMP_Text actionText;

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
                statsText.SetText(equipment.GetStatsStringBuilder());
                if (TokenBrowser.Instance.SelectedToken is HeroToken heroToken)
                {
                    bool trinket = equipment is Scriptable.Trinket;
                    actionText.SetText(equipment is Scriptable.Weapon weapon && heroToken.Scriptable.AttackType == weapon.AttackType ||
                                       equipment is Scriptable.Armor armor && heroToken.Scriptable.ArmorType == armor.ArmorType ||
                                       trinket
                        ? heroToken.ActionPoints > 0 
                            ? trinket 
                                ? "<color=green>Double click to equip (hold shift to equip in second slot)" 
                                : "<color=green>Double click to equip (costs 1 ACT)" 
                            : "<color=red>Selected hero has no ACT left to equip this"
                        : "<color=red>Selected hero cannot equip this" );
                    actionText.gameObject.SetActive(true);
                } else actionText.gameObject.SetActive(false);
            } else if (item is Scriptable.Consumable && 
                       TokenBrowser.Instance.SelectedToken is HeroToken heroToken)
            {
                actionText.SetText(heroToken.ActionPoints > 0 
                    ? "<color=green>Double click to use (costs 1 ACT)" 
                    : "<color=red>Selected hero has no ACT left to use this");
                actionText.gameObject.SetActive(true);
            }
            else actionText.gameObject.SetActive(false);

            statsText.gameObject.SetActive(isEquipment);
            
            int price = item.Price;
            titleText.SetText(item.Name);
            categoryText.SetText(item.CategoryName);
            priceText.SetText(amount == 1 ? $"{price}g" : $"{price}g ({price * amount}g)");
            descriptionText.SetText(item.Description.Equals(string.Empty) ? "No description" : item.Description);
            gameObject.SetActive(true);
            statsText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            descriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.15f);
        }
    }
}