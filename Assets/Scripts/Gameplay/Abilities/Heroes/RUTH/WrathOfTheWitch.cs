using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class WrathOfTheWitch : ActiveAbility, IEffectApplier
    {
        [SerializeField] private int diceAmount;
        [SerializeField] private WrathOfTheWitchBuffEffect wrathOfTheWitchBuffEffect;
        [SerializeField] private int buffDuration;

        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }
        
        public override async UniTask Cast(IInteractable target)
        {
            if (target is not IUncontrollableToken creature) return;
            
            DiceUtil.CaclulateMagicDiceThrow(diceAmount, Caster.MagicDiceSet, Caster.SpellPower, 
                out int damage, out int energy, out int[] sides);
            await DiceManager.ThrowReplay(Caster.MagicDiceSet, diceAmount, sides);
            EnergyManager.Instance.AddEnergy(Caster, energy);
            
            creature.Damage(GlobalDefinitions.ShadowDamageType, damage, Caster);
            creature.BuffManager.ApplyEffect(this, wrathOfTheWitchBuffEffect, buffDuration);
        }

        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);
        public IToken Token => Caster;
    }
}