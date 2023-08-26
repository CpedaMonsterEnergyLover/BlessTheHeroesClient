using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities.Hunter
{
    public class SummonHound : InstantAbility
    {
        [SerializeField] private Scriptable.Creature hound;
        
        public override UniTask Cast(IInteractable _)
        {
            IToken token = GlobalDefinitions.CreateCreatureToken(hound);
            Token.TokenCard.AddToken(token, resetPosition: true, instantly: false);
            Debug.Log("Summon hound");
            return default;
        }
    }
}