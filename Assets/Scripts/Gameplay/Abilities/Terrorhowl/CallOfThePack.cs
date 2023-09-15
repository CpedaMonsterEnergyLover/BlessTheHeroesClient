﻿using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Interaction;
using Gameplay.Tokens;
using UnityEngine;
using Util;
using Util.Patterns;

namespace Gameplay.Abilities.Terrorhowl
{
    public class CallOfThePack : AutoAbility
    {
        [SerializeField] private Scriptable.Creature creatureToSpawn;
        
        protected override void OnTokenSet(IToken token) { }

        public override async UniTask Cast(IInteractable target)
        {
            if (target is not Card center) return;

            foreach (var card in GetCardsForSpawn(center)
                         .Where(card => card.HasSpaceForCreature()))
            {
                await card.AddTokenAsync(GlobalDefinitions.CreateCreatureToken(creatureToSpawn));
            }
        }

        public override bool GetTarget(out IInteractable target)
        {
            target = Caster.TokenCard;
            var cards = GetCardsForSpawn(Caster.TokenCard);
            return CountPossibleSpawnAmount(cards) > 2;
        }

        private int CountPossibleSpawnAmount(List<Card> cards)
            => cards.Sum(card => 8 - card.CreaturesAmount);

        private List<Card> GetCardsForSpawn(Card center)
        {
            List<Card> cards = new();
            PatternSearch.IteratePlus(center.GridPosition, 1, pos =>
            {
                if (FieldManager.GetCard(pos, out Card card) &&
                    card.IsOpened &&
                    card.HasSpaceForCreature())
                {
                   cards.Add(card); 
                }
            }, false);
            return cards;
        }
    }
}