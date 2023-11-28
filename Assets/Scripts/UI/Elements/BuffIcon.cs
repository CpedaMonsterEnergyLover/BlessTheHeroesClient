using Gameplay.BuffEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class BuffIcon : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text durationText;
        [SerializeField] private TMP_Text stacksText;

        private BuffEffect currentEffect;
        
        public void SetBuff(BuffEffect buffEffect)
        {
            if (currentEffect is not null)
            {
                currentEffect.OnDurationChanged -= OnDurationChanged;
                currentEffect.OnStatusChanged -= OnStatusChanged;
            }
            
            if (buffEffect is null || buffEffect.Duration == 0)
            {
                gameObject.SetActive(false);
                currentEffect = null;
                return;
            }

            if (buffEffect is StackableBuffEffect stackable)
            {
                UpdateStacksText(stackable.Stacks);
                stackable.OnStacksUpdated += UpdateStacksText;
            } else UpdateStacksText(0);

            currentEffect = buffEffect;
            icon.sprite = buffEffect.Scriptable.Icon;
            UpdateDurationText(buffEffect.Duration);
            gameObject.SetActive(true);
            buffEffect.OnDurationChanged += OnDurationChanged;
            currentEffect.OnStatusChanged += OnStatusChanged;
        }

        private void OnDurationChanged(BuffEffect effect)
        {
            UpdateDurationText(effect.Duration);
            if (effect.Duration == 0) RemoveEffect();
        }
        
        private void UpdateDurationText(int duration) => durationText.text = duration == int.MaxValue ? "" : duration.ToString();

        private void UpdateStacksText(int stacks) => stacksText.text = stacks <= 1 ? "" : stacks.ToString();

        private void OnStatusChanged(BuffEffect effect)
        {
            if(!effect.enabled) RemoveEffect();
        }

        private void RemoveEffect()
        {
            gameObject.SetActive(false);
            currentEffect.OnDurationChanged -= OnDurationChanged;
            currentEffect.OnStatusChanged -= OnStatusChanged;
            if (currentEffect is StackableBuffEffect stackable)
                stackable.OnStacksUpdated -= UpdateStacksText;
            currentEffect = null;
        }
    }
}