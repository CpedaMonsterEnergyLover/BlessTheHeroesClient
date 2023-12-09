﻿using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Abilities;
using Gameplay.Dice;
using Gameplay.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        
        
        public void SetAbility(Ability ability, bool withAnimation = true)
        {
            if(ability is null)
            {
                gameObject.SetActive(false);
                return;
            }

            titleText.SetText(ability.Title);
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
                string mText = castable.Energycost > 0
                    ? $"{castable.Energycost} Energy"
                    : $"{castable.Manacost} Mana";
                manaText.SetText(mText + (castable.Healthcost > 0 ? $", {castable.Healthcost} Health" : string.Empty));
                manaText.gameObject.SetActive(true);
                cooldownText.SetText(castable.BaseCooldown == 0
                    ? "No cooldown"
                    : $"{castable.BaseCooldown} Turn{(castable.BaseCooldown > 1 ? "s" : string.Empty)} Cooldown");
            }
            
            // State text
            if(TokenBrowser.Instance.SelectedToken is IControllableToken hero && ability is CastableAbility castable1)
            {
                stateText.SetText(castable1.RequiresAct && hero.ActionPoints <= 0
                    ? "<color=red>Not enough ACT to cast"
                    : castable1.CurrentCooldown > 0
                        ? "<color=red>Ability is on cooldown" 
                        : hero.CurrentMana < castable1.Manacost 
                            ? "<color=red>Not enough mana to cast" 
                            : castable1.Energycost > 0 && EnergyManager.Instance.Energy < castable1.Energycost
                                ? "<color=red>Not enough energy to cast" 
                                : string.Empty);
                
                stateText.gameObject.SetActive(!stateText.text.Equals(string.Empty));
            } else stateText.gameObject.SetActive(false);

            gameObject.SetActive(true);
            detailDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            literalDescriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            if(withAnimation)
            {
                PlayAnimation();
                UpdatePivotPosition().Forget();
            }
            else 
                pivot.localScale = Vector3.one;
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