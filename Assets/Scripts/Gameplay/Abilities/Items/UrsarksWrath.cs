using Cysharp.Threading.Tasks;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class UrsarksWrath : ActiveAbility
    {
        [SerializeField] private int maxDamage;
        
        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not IToken token) return;
            int damage = Mathf.Clamp(Caster.MaxHealth - Caster.CurrentHealth, 0, maxDamage);
            token.Damage(GlobalDefinitions.PhysicalDamageType, damage, aggroReceiver: Caster.IAggroManager);
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }
    }
}