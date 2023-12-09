using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities
{
    public class MassDispel : ActiveAbility
    {
        [SerializeField] private ParticleSystem particles;
        
        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not Card card) return;
            
            foreach (IUncontrollableToken uncontrollable in card.Creatures)
            {
                BuffManager buffManager = uncontrollable.BuffManager;
                foreach (BuffEffect buffEffect in buffManager.ActiveBuffs
                             .Where(b => b.Scriptable.Dispellable && b.Scriptable.EffectType is BuffEffectType.Magic))
                    buffManager.RemoveExact(buffEffect);
            }
            
            foreach (IControllableToken controllable in card.Heroes)
            {
                BuffManager buffManager = controllable.BuffManager;
                foreach (BuffEffect buffEffect in buffManager.ActiveDebuffs
                             .Where(b => b.Scriptable.Dispellable && b.Scriptable.EffectType is BuffEffectType.Magic or BuffEffectType.Curse))
                    buffManager.RemoveExact(buffEffect);
            }
            
            particles.transform.position = card.transform.position + new Vector3(0, 0.2f, 0);
            particles.Play();
            await UniTask.WaitUntil(() => !particles.isPlaying);
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is Card { IsOpened: true };
        }
    }
}