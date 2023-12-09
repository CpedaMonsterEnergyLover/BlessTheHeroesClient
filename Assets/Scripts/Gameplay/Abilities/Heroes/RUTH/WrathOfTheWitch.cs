using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Dice;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;

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
            
            Util.DiceUtil.CaclulateMagicDiceThrow(diceAmount, Caster.MagicDiceSet, Caster.SpellPower, 
                out int damage, out int energy, out int[] sides);
            await DiceManager.ThrowReplay(Caster.MagicDiceSet, diceAmount, sides);
            EnergyManager.Instance.AddEnergy(Caster, energy);
            
            creature.Damage(damage, aggroSource: Caster.IAggroManager);
            creature.BuffManager.ApplyEffect(this, wrathOfTheWitchBuffEffect, buffDuration);
        }

        public override bool ValidateTarget(IInteractable target) => ValidateEnemy(target);
    }
}