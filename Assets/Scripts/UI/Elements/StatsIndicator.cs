using TMPro;
using UnityEngine;

namespace UI.Elements
{
    public class StatsIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text spellText;
        [SerializeField] private TMP_Text attackText;
        [SerializeField] private TMP_Text defenseText;

        public void SetStats(int speed, int spell, int attack, int defense)
        {
            speedText.SetText(speed.ToString());
            spellText.SetText(spell.ToString());
            attackText.SetText(attack.ToString());
            defenseText.SetText(defense.ToString());
        }
    }
}