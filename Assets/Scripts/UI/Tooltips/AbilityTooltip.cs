using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.Tokens;
using TMPro;
using UI.Browsers;
using UnityEngine;

namespace UI.Tooltips
{
    public class AbilityTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform pivot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text stats;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private TMP_Text manaText;

        
        
        public void SetAbility(Ability ability, bool withAnimation = true)
        {
            if(ability is null)
            {
                gameObject.SetActive(false);
                return;
            }

            titleText.SetText(ability.Title);
            stats.SetText(ability.StatDescription);
            description.SetText(ability.Description);
            
            // Mana and cooldown
            if(ability is PassiveAbility)
            {
                cooldownText.SetText("Passive ability");
                manaText.gameObject.SetActive(false);
            }
            else if(ability is CastableAbility castable)
            {
                string mText = castable.Energycost > 0
                    ? $"<color=orange>{castable.Energycost} Energy</color>"
                    : $"<color=#00ffffff>{castable.Manacost} Mana</color>";
                manaText.SetText(mText + (castable.Healthcost > 0 ? $", <color=red>{castable.Healthcost} Health</color>" : string.Empty));
                manaText.gameObject.SetActive(true);
                cooldownText.SetText(castable.BaseCooldown == 0
                    ? "No cooldown"
                    : $"{castable.BaseCooldown} Turn{(castable.BaseCooldown > 1 ? "s" : string.Empty)} Cooldown");
            }
            
            // State text
            if(TokenBrowser.SelectedToken is IControllableToken hero && ability is CastableAbility castable1)
            {
                actionText.SetText(castable1.RequiresAct && hero.ActionPoints <= 0
                    ? "<color=red>Not enough ACT to cast"
                    : castable1.CurrentCooldown > 0
                        ? "<color=red>Ability is on cooldown" 
                        : hero.CurrentMana < castable1.Manacost 
                            ? "<color=red>Not enough mana to cast" 
                            : castable1.Energycost > 0 && EnergyManager.Instance.Energy < castable1.Energycost
                                ? "<color=red>Not enough energy to cast" 
                                : string.Empty);
                
                actionText.gameObject.SetActive(!actionText.text.Equals(string.Empty));
            } else actionText.gameObject.SetActive(false);

            gameObject.SetActive(true);
            if (withAnimation)
                PlayAnimation();
            else
                pivot.localScale = Vector3.one;
        }
        private void PlayAnimation()
        {
            pivot.localScale = Vector3.zero;
            pivot.DOScale(Vector3.one, 0.15f);
        }
    }
}