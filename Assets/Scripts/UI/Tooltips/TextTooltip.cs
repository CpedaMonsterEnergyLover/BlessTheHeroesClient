using TMPro;
using UnityEngine;

namespace UI.Tooltips
{
    public class TextTooltip : MonoBehaviour
    {
        public static TextTooltip Instance { get; private set; }
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform shadow;

        private TextTooltip() => Instance = this;

        
        
        public void SetText(string message)
        {
            text.SetText(message);
            float shadowWidth = Mathf.Max(text.preferredWidth + 54, 90);
            float shadowHeight = text.preferredHeight;
            shadow.sizeDelta = new Vector2(shadowWidth, shadowHeight);
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}