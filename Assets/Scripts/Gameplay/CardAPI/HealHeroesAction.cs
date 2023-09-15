using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace CardAPI
{
    [System.Serializable]
    public class HealHeroesAction : CardAction
    {
        [Header("HealHeroes")]
        [SerializeField] private TargetSelector selector;
        [SerializeField] private int amount;

        
        
        public override string Description
            => $"restores {amount} health to {(selector == TargetSelector.Card ? " all heroes on that location." : " a hero.")}";

        public override void Execute(Card card, IControllableToken executor, object data = null)
        {
            switch (selector)
            {
                case TargetSelector.Card:
                    foreach (HeroToken hero in card.Heroes) 
                        hero.Heal(amount);
                    break;
                case TargetSelector.Executor:
                    executor.Heal(amount);
                    break;
            }
        }
    }
}