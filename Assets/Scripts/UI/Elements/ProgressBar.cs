using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private bool hasValueText = true;
        [SerializeField, ConditionalField(nameof(hasValueText), false, true)]
        private TMP_Text valueText;

        private float maxValue;
        private float currentValue;


        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void UpdateValue(float current, float max)
        {
            currentValue = current;
            maxValue = max;
            UpdateImage();
            UpdateText();
        }

        private void UpdateImage() => fillImage.fillAmount = Mathf.Clamp01(currentValue / maxValue);

        private void UpdateText()
        {
            if(!hasValueText) return;
            valueText.SetText($"{currentValue}/{maxValue}");
        }
    }
}