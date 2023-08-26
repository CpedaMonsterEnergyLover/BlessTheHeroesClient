using Gameplay.Events;
using TMPro;
using UnityEngine;

namespace UI.Inspection
{
    public class EventSection : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        public void SetEvent(CardEvent cardEvent)
        {
            if (cardEvent is null)
            {
                gameObject.SetActive(false);
                return;
            }

            text.SetText(cardEvent.Name);
            gameObject.SetActive(true);
        }
        
        public void SetUnknown()
        {
            text.SetText("A random event will occur when a hero moves on that card.");
            gameObject.SetActive(true);
        }
    }
}