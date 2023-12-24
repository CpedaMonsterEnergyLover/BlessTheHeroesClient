using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class Hex : ActiveAbility
    {
        [SerializeField] private Scriptable.Creature frog;
        
        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if(target is not CreatureToken creature) return;

            Card targetCard = creature.Card;
            /*Caster.TokenCard.RemoveTokenWithoutUpdate(creature);
            Destroy(creature.gameObject);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);*/

            await creature.Despawn();
            CreatureToken token = GlobalDefinitions.CreateCreatureToken(frog);
            targetCard.AddToken(token, resetPosition: true, instantly: false);
        }

        public override bool ValidateTarget(IInteractable target) => ValidateCreature(target);
    }
}