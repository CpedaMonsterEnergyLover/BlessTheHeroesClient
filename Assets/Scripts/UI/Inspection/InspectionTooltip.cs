using DG.Tweening;
using Gameplay.Cards;
using UnityEngine;

namespace UI.Inspection
{
    public class InspectionTooltip : MonoBehaviour
    {
        [SerializeField] private CardSection cardSection;
        [SerializeField] private ActionSection actionSection;
        [SerializeField] private EventSection eventSection;

        private Tween tween;
        
        public void InspectCard(Card card)
        {
            cardSection.SetCard(card);
            actionSection.SetAction(card.Scriptable.CardAction);
            eventSection.SetEvent(null);
            Toggle(true);
        }
        
        public void Toggle(bool state)
        {
            if(tween is not null) tween.Kill();
            if(state)
            {
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
            }
            tween = transform.DOScale(state ? Vector3.one : Vector3.zero, 0.15f).OnComplete(() =>
            {
                if(!state) transform.gameObject.SetActive(false);
                tween = null;
            });
        }
    }
}