using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class TargetHealAbility : ActiveAbility
    {
        [SerializeField] private int heal;
        
        public override bool ValidateTarget(IInteractable target) => ValidateAlly(target);

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return;
            
            token.Heal(heal, aggroManager: Caster.IAggroManager);
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }
    }
}