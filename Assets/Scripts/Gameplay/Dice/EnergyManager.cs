using Gameplay.GameCycle;
using Gameplay.Tokens;
using TMPro;
using UI;
using UnityEngine;

namespace Gameplay.Dice
{
    public class EnergyManager : MonoBehaviour
    {
        public static EnergyManager Instance { get; private set; }
        
        [SerializeField] private TMP_Text energyText;
        [SerializeField] private int energyOnStart;

        public int Energy { get; private set; }


        
        private void OnEnable() => TurnManager.OnPlayersTurnStarted += OnPlayersTurnStarted;

        private void OnDisable() => TurnManager.OnPlayersTurnStarted -= OnPlayersTurnStarted;



        // Class Methods
        private EnergyManager() => Instance = this;

        private void UpdateEnergyText() => energyText.SetText(Energy.ToString());

        public bool TryDrainEnergy(int amount)
        {
            if (Energy - amount >= 0)
            {
                Energy -= amount;
                UpdateEnergyText();
                return true;
            }
            return false;
        }

        public void AddEnergy(IToken source, int amount)
        {
            if(source is not HeroToken) return;
            Energy += amount;
            UpdateEnergyText();
        }

        private void OnPlayersTurnStarted()
        {
            Energy = energyOnStart;
            UpdateEnergyText();
        }
    }
}