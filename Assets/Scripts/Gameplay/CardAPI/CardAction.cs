using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;

namespace CardAPI
{
    [System.Serializable]
    public abstract class CardAction
    {
        [SerializeField] private string name;

        public string Name => name;


        public abstract string Description { get; }
        public abstract void Execute(Card card, HeroToken executor, object data = null);
    }
}