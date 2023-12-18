using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace Gameplay.Abilities
{
    public class VoodooDoll : ActiveAbility, IEffectApplier
    {
        [SerializeField] private Scriptable.Creature tokenToSpawn;
        [SerializeField] private VoodooDollBuffEffect voodooDollBuffEffect;
        
        public IToken LastTarget { get; private set; }
        
        public override void OnCastStart()
        {
        }

        public override void OnCastEnd()
        {
        }

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not IToken token) return;

            LastTarget = token;
            IToken spawned = token is IUncontrollableToken
                ? GlobalDefinitions.CreateCreatureToken(tokenToSpawn)
                : GlobalDefinitions.CreateCompanionToken(tokenToSpawn);
            Caster.TokenCard.AddToken(spawned, resetPosition: true, instantly: false);
            spawned.BuffManager.ApplyEffect(this, voodooDollBuffEffect, int.MaxValue);
        }

        public override bool ValidateTarget(IInteractable target)
        {
            return target is IToken token && !token.ScriptableToken.Equals(tokenToSpawn) &&
                   (target is IUncontrollableToken creature
                       ? creature.CanBeTargeted && Caster.TokenCard.HasSpaceForCreature()
                       : target is IControllableToken && Caster.TokenCard.HasSpaceForHero());
        }
    }
}