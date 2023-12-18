using Gameplay.GameCycle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browsers
{
    public class TurnBrowser : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button passButton;

        public void UpdateText(int turnsCounter, TurnStage turnStage)
            => text.SetText($"Turn {turnsCounter} - {turnStage}");

        public void SetPassButtonEnabled(bool isEnabled) => passButton.gameObject.SetActive(isEnabled);
    }
}