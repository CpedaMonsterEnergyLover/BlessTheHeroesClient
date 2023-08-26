using Gameplay.GameField;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inspection
{
    public class CardSection : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text literalDescription;
        [SerializeField] private TMP_Text detailDescription;

        public void SetCard(Card card)
        {
            var scriptable = card.Scriptable;
            image.sprite = scriptable.Sprite;
            title.SetText(scriptable.Name);
            literalDescription.SetText(scriptable.LiteralDescription.Equals(string.Empty) 
                ? "No description" 
                : scriptable.LiteralDescription);
            string detail = scriptable.DetailDescription;
            detailDescription.SetText(detail);
            detailDescription.gameObject.SetActive(!detail.Equals(string.Empty));
            detailDescription.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            literalDescription.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        }
    }
}