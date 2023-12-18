using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Cards
{
    public abstract class CardAction : MonoBehaviour
    {
        [SerializeField] private new string name;
        [SerializeField] protected bool useOpeningRoll;

        public bool UseOpeningRoll => useOpeningRoll; 
        public string Name => name;
        public abstract string Description { get; }
        public abstract void Execute(Card card, IControllableToken executor, int rollResult = 0);
    }
}