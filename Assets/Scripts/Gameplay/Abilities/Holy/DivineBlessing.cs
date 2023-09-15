using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using Gameplay.Tokens.Buffs;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class DivineBlessing : InstantAbility
    {
        [SerializeField] private BuffEffect effectToApply;
        [SerializeField] private int duration;

        
        
        protected override void OnTokenSet(IToken token)
        {
        }

        public override UniTask Cast(IInteractable target)
        {
            if (target is not Card card) return default;
            
            foreach (IControllableToken controllable in card.Heroes)
            {
                controllable.BuffManager.ApplyEffect(effectToApply, duration);
            }
            
            return default;
        }
    }
}