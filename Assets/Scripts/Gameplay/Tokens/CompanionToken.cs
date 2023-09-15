using Gameplay.GameField;
using Scriptable;

namespace Gameplay.Tokens
{
    public class CompanionToken : ControllableToken<Creature>
    {
        public override DiceSet AttackDiceSet => Scriptable.OverrideAttackDice(out DiceSet dice) ? dice : FieldManager.MonsterAttackDice;
        public override DiceSet MagicDiceSet => Scriptable.OverrideMagicDice(out DiceSet dice) ? dice : FieldManager.MonsterMagicDice;
        public override DiceSet DefenseDiceSet => Scriptable.OverrideDefenseDice(out DiceSet dice) ? dice : FieldManager.MonsterDefenseDice;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => Scriptable.DefenseDiceAmount;
        protected override bool CanInteractWithCards => false;
        public override bool CanClick => true;


        protected override void Init()
        {
            base.Init();
            ActionPoints = 2;
        }


        protected override void OnDeath()
        {
        }
    }
}