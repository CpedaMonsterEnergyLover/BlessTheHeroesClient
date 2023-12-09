using Gameplay.Cards;
using TMPro;
using UnityEngine;

namespace UI.Inspection
{
    public class ActionSection : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        
        
        public void SetAction(CardAction cardAction)
        {
            if (cardAction is null)
            {
                gameObject.SetActive(false);
                return;
            }

            text.SetText($"{cardAction.Name}: {cardAction.Description}");
            gameObject.SetActive(true);
        }
    }
}