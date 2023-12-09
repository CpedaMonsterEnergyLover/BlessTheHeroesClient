using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util.Enums;

namespace Gameplay.Abilities
{
    public class DispelTargetAbility : ActiveAbility
    {
        [SerializeField] private BuffEffectType[] toDispel;
        [SerializeField] private int maxDispelledAmount;
        [SerializeField] private bool dispelAllies;
        [SerializeField] private bool dispelEnemies;
        
        public override bool ValidateTarget(IInteractable target) 
            => dispelAllies && ValidateAlly(target) || dispelEnemies && ValidateEnemy(target);

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not IToken token) return;

            BuffManager buffManager = token.BuffManager;
            foreach (BuffEffectType effectType in toDispel)
            {
                var effects = buffManager.FindEffectsOfType(effectType);
                int len = effects.Length;
                for (int i = 0; i < len && i < maxDispelledAmount; i++) 
                    buffManager.RemoveExact(effects[i]);
            }
        }

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }
    }
}