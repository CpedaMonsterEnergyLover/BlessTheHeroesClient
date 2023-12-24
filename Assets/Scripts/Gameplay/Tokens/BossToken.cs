using Cysharp.Threading.Tasks;
using Gameplay.Cards;
using Scriptable;
using Scriptable.AttackVariations;


namespace Gameplay.Tokens
{
    public class BossToken : UncontrollableToken<Boss>
    {
        protected override int DefaultActionPoints => 2;
        public override DiceSet AttackDiceSet => Scriptable.AttackDice;
        public override DiceSet MagicDiceSet => Scriptable.MagicDice;
        public override DiceSet DefenseDiceSet => Scriptable.DefenseDice;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => (int) Scriptable.ArmorType;
        public override BaseAttackVariation AttackVariation => Scriptable.AttackVariation;
        public override DamageType DamageType => Scriptable.DamageType;
        protected override float SharedLootDropModifier => 1;



        protected override async UniTask Walk(Card card)
        {
            /*
            if (card.HasBoss) 
                return;
            if (!CanWalk && !ConsumeActionPointForMovement()) 
                return;
            */

            Card.PushOrDespawnCreatures();
            await base.Walk(card);
        }
    }
}