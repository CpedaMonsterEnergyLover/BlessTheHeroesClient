using Cysharp.Threading.Tasks;
using Gameplay.Tokens;
using MyBox;
using UnityEngine;
using Util;

namespace Gameplay.Cards
{
    public class SpawnCreaturesCardAction : CardAction
    {
        [Header("SpawnCreature")]
        [SerializeField] private Scriptable.Creature creature;
        [SerializeField, ConditionalField(nameof(useOpeningRoll), false, false), Range(1, 8)]
        private int amount;
        
        public override string Description =>  $"spawns {(UseOpeningRoll ? "D" : amount)} x {creature.Name}";
        
        
        
        public override void Execute(Card card, IControllableToken executor, int rollResult = 0) 
            => ExecuteAsync(card, UseOpeningRoll ? rollResult : amount).Forget();

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