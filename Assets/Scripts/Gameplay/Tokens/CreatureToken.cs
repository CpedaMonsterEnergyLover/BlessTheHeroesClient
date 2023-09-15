using Gameplay.GameField;
using Scriptable;

namespace Gameplay.Tokens
{
    public class CreatureToken : UncontrollableToken<Creature>
    {
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => Scriptable.DefenseDiceAmount;
        public override DiceSet AttackDiceSet => Scriptable.OverrideAttackDice(out DiceSet dice) ? dice : FieldManager.MonsterAttackDice;
        public override DiceSet MagicDiceSet => Scriptable.OverrideMagicDice(out DiceSet dice) ? dice : FieldManager.MonsterMagicDice;
        public override DiceSet DefenseDiceSet => Scriptable.OverrideDefenseDice(out DiceSet dice) ? dice : FieldManager.MonsterDefenseDice;

        
        
    }
}