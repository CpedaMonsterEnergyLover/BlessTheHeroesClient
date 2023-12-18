using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Gameplay.Interaction;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class SelfAggroChangeAbility : InstantAbility
    {
        [SerializeField] private int aggroChange;
        
        public override bool ValidateTarget(IInteractable target) => target is Card || target.Equals(Caster);

        public override async UniTask Cast(IInteractable target)
        {
            Caster.IAggroManager.ChangeClusterAggro(aggroChange);
        }
    }
}