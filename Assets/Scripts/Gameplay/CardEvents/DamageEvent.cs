using Gameplay.Cards;
using Gameplay.Tokens;
using Scriptable;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Events
{
    public class DamageEvent : CardEvent
    {
        [SerializeField] private TargetSelector targetSelector;
        [SerializeField] private DamageType damageType;
        [SerializeField] private int damage;
        
        public override void Execute(Card card, HeroToken executor)
        {
            
            switch (targetSelector)
            {
                case TargetSelector.Card:
                    foreach (HeroToken hero in card.Heroes) 
                        hero.Damage(damageType, damage);
                    break;
                case TargetSelector.Executor:
                    executor.Damage(damageType, damage);
                    break;
            }
        }
    }
}