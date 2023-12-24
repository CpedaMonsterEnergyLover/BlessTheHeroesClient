using Gameplay.Tokens;
using Scriptable;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Cards
{
    public class HealCardAction : CardAction
    {
        [Header("HealHeroes")]
        [SerializeField] private TargetSelector selector;
        [SerializeField] private int amount;
        [SerializeField] private DamageType healType;
        
        public override string Description
            => $"restores {amount} health to {(selector == TargetSelector.Card ? " all heroes on that location." : " a hero.")}";

        
        
        public override void Execute(Card card, IControllableToken executor, int rollResult = 0)
        {
            switch (selector)
            {
                case TargetSelector.Card:
                    foreach (IControllableToken hero in card.Heroes) 
                        hero.Heal(healType, amount, null);
                    break;
                case TargetSelector.Executor:
                    executor.Heal(healType, amount, null);
                    break;
            }
        }
    }
}