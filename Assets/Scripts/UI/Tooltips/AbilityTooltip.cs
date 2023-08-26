using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Enums;

namespace UI.Tooltips
{
    public class AbilityTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform pivot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailDescriptionText;
        [SerializeField] private TMP_Text literalDescriptionText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text cooldownText;
        [SerializeField] private TMP_Text manaText;

        
        
        public void SetAbility(Ability ability)
        {
            if(ability is null)
            {
                gameObject.SetActive(false);
                return;
            }

            titleText.SetText(ability.Name);
            detailDescriptionText.SetText(ability.DetailDescription);
            literalDescriptionText.SetText(ability.LiteralDescription);
            
            // Mana and cooldown
            if(ability is PassiveAbility)
            {
                cooldownText.SetText("Passive ability");
                manaText.gameObject.SetActive(false);
            }
            else if(ability is CastableAbility castable)
            {
                manaText.SetText($"{castable.Manacost} Mana");
                manaText.gameObject.SetActive(true);
                cooldownText.SetText(castable.BaseCooldown == 0
                    ? "No cooldown"
                    : $"{castable.BaseCooldown} Turn{(castable.BaseCooldown > 1 ? "s" : string.Empty)} Cooldown");
            }
            
            // State text
            if(TokenBrowser.Instance.SelectedToken is HeroToken hero && ability is CastableAbility castable1)
            {
                stateText.SetText(hero.ActionPoints <= 0
                    ? "<color=red>Not enough ACT"
                    : castable1.CurrentCooldown > 0
                        ? "<color=red>Ability is on cooldown" 
                        : hero.CurrentMana < castable1.Manacost 
                            ? "<color=red>Not enough mana" 
                            : string.Empty);
                stateText.gameObject.SetActive(!stateText.text.Equals(string.Empty));
            } else stateText.gameObject.SetActive(false);

            gameObject.SetActive(true);
            detailDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            literalDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            PlayAnimation();
            UpdatePivotPosition().Forget();
        }

        private async UniTaskVoid UpdatePivotPosition()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            Vector3 pos = pivot.localPosition;
            pivot.localPosition = new Vector3(pos.x, pivot.sizeDelta.y / 2f, pos.z);
        }
        
        private void PlayAnimation()
        {
            pivot.localScale = Vector3.zero;
            pivot.DOScale(Vector3.one, 0.15f);
        }
    }
}