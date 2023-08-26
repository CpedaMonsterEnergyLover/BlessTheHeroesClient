using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltips
{
    public class EquipmentTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform pivot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text categoryText;

        public void SetItem(Scriptable.Equipment item)
        {
            if(item is null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            int price = item.Price;
            statsText.SetText(item.GetStatsStringBuilder());
            titleText.SetText(item.Name);
            categoryText.SetText(item.CategoryName);
            priceText.SetText($"{price}g");
            descriptionText.SetText(item.Description.Equals(string.Empty) ? "No description" : item.Description);
            gameObject.SetActive(true);
            statsText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            descriptionText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            PlayAnimation();
            UpdatePivotPosition().Forget();
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