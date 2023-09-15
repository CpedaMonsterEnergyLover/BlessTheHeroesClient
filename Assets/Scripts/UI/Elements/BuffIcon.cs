using Gameplay.Tokens.Buffs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class BuffIcon : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text durationText;

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

            currentEffect = buffEffect;
            icon.sprite = buffEffect.Scriptable.Icon;
            durationText.text = buffEffect.Duration.ToString();
            gameObject.SetActive(true);
            buffEffect.OnDurationChanged += OnDurationChanged;
            currentEffect.OnStatusChanged += OnStatusChanged;
        }

        private void OnDurationChanged(BuffEffect effect)
        {
            durationText.text = effect.Duration.ToString();
            if (effect.Duration == 0) RemoveEffect();
        }

        private void OnStatusChanged(BuffEffect effect)
        {
            if(!effect.enabled) RemoveEffect();
        }

        private void RemoveEffect()
        {
            gameObject.SetActive(false);
            currentEffect.OnDurationChanged -= OnDurationChanged;
            currentEffect.OnStatusChanged -= OnStatusChanged;
            currentEffect = null;
        }
    }
}