using Gameplay.Abilities;
using TMPro;
using UnityEngine;

namespace UI.Elements
{
    public class AbilityResourceIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject mana;
        [SerializeField] private GameObject health;
        [SerializeField] private GameObject energy;
        [SerializeField] private TMP_Text manaText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text energyText;

        public void SetAbility(Ability ability)
        {
            if (ability is CastableAbility castable)
            {
                manaText.SetText(castable.Manacost.ToString());
                healthText.SetText(castable.Healthcost.ToString());
                energyText.SetText(castable.Energycost.ToString());
                mana.SetActive(castable.Manacost != 0);
                health.SetActive(castable.Healthcost != 0);
                energy.SetActive(castable.Energycost != 0);
            }
            else
            {
                mana.SetActive(false);
                health.SetActive(false);
                energy.SetActive(false);
            }
        }
    }
}