using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                            : "<color=red>Not enough ACT to equip"
                        : "<color=red>Selected hero cannot equip this" );
                    actionText.gameObject.SetActive(true);
                } else actionText.gameObject.SetActive(false);
            } else if (item is Scriptable.Consumable && 
                       TokenBrowser.Instance.SelectedToken is HeroToken heroToken)
            {
                actionText.SetText(heroToken.ActionPoints > 0 
                    ? "<color=green>Double click to use (costs 1 ACT)" 
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
            statsText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            descriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            PlayAnimation();
            // UpdatePivotPosition().Forget();
        }

        /*
        private async UniTaskVoid UpdatePivotPosition()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);/*
            Vector3 pos = pivot.localPosition;
            pivot.localPosition = new Vector3(pos.x, pivot.sizeDelta.y / 2f, pos.z);#1#
            abilityTooltip.GetComponent<ContentSizeFitter>().SetLayoutVertical();

        }*/
        
        private void PlayAnimation()
        {
            pivot.localScale = Vector3.zero;
            pivot.DOScale(Vector3.one, 0.15f);
        }
    }
}