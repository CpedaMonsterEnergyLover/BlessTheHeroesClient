using System;
using Cysharp.Threading.Tasks;
using Gameplay.GameField;
using Gameplay.Tokens;
using UnityEngine;
using Util;

namespace CardAPI
{
    [Serializable]
    public class SpawnCreaturesAction : CardAction
    {
        [Header("SpawnCreature")]
        [SerializeField] private Scriptable.Creature creature;
        [SerializeField] private int amount;
        
        
        
        public override string Description 
            =>  $"spawns {(amount == -1 ? "D" : amount)} x {creature.Name}";
        
        public override void Execute(Card card, IControllableToken executor, object data = null)
        {
            int amountToSpawn = amount == -1 
                ? data is int value
                    ? value
                    : 0
                : amount;
            ExecuteAsync(card, amountToSpawn).Forget();
        }

        private async UniTaskVoid ExecuteAsync(Card card, int amountToSpawn)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                if(!card.HasSpaceForCreature()) return;
                
                await card.AddTokenAsync(GlobalDefinitions.CreateCreatureToken(creature));
            }
        }
    }
}