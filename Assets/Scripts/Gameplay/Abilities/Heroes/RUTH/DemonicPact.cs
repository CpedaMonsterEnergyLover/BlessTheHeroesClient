using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class DemonicPact : InstantAbility
    {
        [SerializeField] private Scriptable.Creature toSummon;
        
        private IControllableToken summonedToken;


        private void OnTokenDestroy(IInteractable interactable)
        {
            interactable.OnDestroyed -= OnTokenDestroy;
            summonedToken = null;
        }

        public override bool ApproveCast(IToken token)
        {
            return base.ApproveCast(token) && 
                   summonedToken is null &&
                   Caster.Card.HasSpaceForHero();
        }

        public override bool ValidateTarget(IInteractable target) => target is Card;

        public override UniTask Cast(IInteractable target)
        {
            summonedToken = GlobalDefinitions.CreateCompanionToken(toSummon);
            summonedToken.OnDestroyed += OnTokenDestroy;
            Caster.Card.AddToken(summonedToken, resetPosition: true, instantly: false);
            return default;
        }
    }
}