using Camera;
using Gameplay.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Interaction;

namespace UI.Interaction
{
    public class InteractionTooltip : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite[] interactionIcons = new Sprite[4];
        
        private new MainCamera camera;
        private RectTransform rectTransform;
        
        
        public void Show(InteractionTooltipData data)
        {
            if (data is null || data.Mode == InteractionMode.None)
            {
                gameObject.SetActive(false);
                return;
            } 

            icon.sprite = interactionIcons[(int) data.Mode];
            icon.color = InteractionColor.Get(data.State);
            titleText.text = data.ActionTitle;
            subtitleText.text = data.ActionSubtitle;
            subtitleText.enabled = data.State switch
            {
                InteractionState.Allow => false,
                InteractionState.Abandon => true,
                _ => false
            };
            
            if(!gameObject.activeInHierarchy) gameObject.SetActive(true);
        }
    }
}