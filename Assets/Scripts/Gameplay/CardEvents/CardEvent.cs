using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Events
{
    public abstract class CardEvent : MonoBehaviour
    {
        [SerializeField] private new string name;

        public string Name => name;
        public abstract void Execute(Card card, HeroToken executor);
    }
}