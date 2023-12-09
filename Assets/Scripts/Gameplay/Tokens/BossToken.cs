using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Gameplay.GameField;
using Scriptable.AttackVariations;


namespace Gameplay.Tokens
{
    public class BossToken : UncontrollableToken<Scriptable.Boss>
    {
        protected override int DefaultActionPoints => 2;
        public override Scriptable.DiceSet AttackDiceSet => Scriptable.AttackDice;
        public override Scriptable.DiceSet MagicDiceSet => Scriptable.MagicDice;
        public override Scriptable.DiceSet DefenseDiceSet => Scriptable.DefenseDice;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => Scriptable.DefenseDiceAmount;
        public override BaseAttackVariation AttackVariation => Scriptable.AttackVariation;


        
        protected override async UniTask Walk(Card card)
        {
            if (card.HasBoss) 
                return;
            if (MovementPoints <= 0 && 
                !ConsumeActionPointForMovement()) 
                return;

            Card.PushOrDespawnCreatures();

            await base.Walk(card);
        }

        protected override float SharedLootDropModifier => 1;
    }
}