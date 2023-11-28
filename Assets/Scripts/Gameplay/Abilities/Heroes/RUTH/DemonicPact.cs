using Cysharp.Threading.Tasks;
using Gameplay.GameField;
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


        private void OnTokenDestroy(IToken t)
        {
            t.OnDestroyed -= OnTokenDestroy;
            summonedToken = null;
        }

        public override bool ApproveCast(IToken token)
        {
            return base.ApproveCast(token) && 
                   summonedToken is null &&
                   Caster.TokenCard.HasSpaceForHero();
        }

        public override bool ValidateTarget(IInteractable target) => target is Card;

        public override UniTask Cast(IInteractable target)
        {
            summonedToken = GlobalDefinitions.CreateCompanionToken(toSummon);
            summonedToken.OnDestroyed += OnTokenDestroy;
            Caster.TokenCard.AddToken(summonedToken, resetPosition: true, instantly: false);
            return default;
        }
    }
}