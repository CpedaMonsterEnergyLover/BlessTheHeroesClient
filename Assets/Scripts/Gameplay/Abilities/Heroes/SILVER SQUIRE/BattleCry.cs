using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.BuffEffects;
using Gameplay.Cards;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util.Patterns;

namespace Gameplay.Abilities
{
    public class BattleCry : InstantAbility, IEffectApplier
    {
        [SerializeField] private BattleCryBuffEffect battleCryBuffEffect;

        public override bool ValidateTarget(IInteractable target) => ValidateOpenedCard(target) || ValidateAlly(target);

        public override async UniTask Cast(IInteractable target)
        {
            int enemiesAmount = Caster.TokenCard.Creatures.Count(c => !c.Dead);
            PatternSearch.IterateArea(Caster.TokenCard.GridPosition, 1, pos =>
            {
                if(!FieldManager.GetCard(pos, out Card card)) return;
                var toBuff = card.Heroes.Where(h => !h.Dead);
                foreach (IControllableToken token in toBuff)
                {
                    for(int i = 0; i < enemiesAmount; i++)
                        token.BuffManager.ApplyEffect(this, battleCryBuffEffect, 1);
                }
            });
        }
    }
}