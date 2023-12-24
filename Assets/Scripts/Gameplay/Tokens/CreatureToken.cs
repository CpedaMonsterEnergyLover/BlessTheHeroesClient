using Gameplay.GameField;
using Scriptable;
using Scriptable.AttackVariations;

namespace Gameplay.Tokens
{
    public class CreatureToken : UncontrollableToken<Creature>
    {
        protected override int DefaultActionPoints => Scriptable.AllowedToAct ? 1 : 0;
        public override DamageType DamageType => Scriptable.DamageType;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => (int) Scriptable.ArmorType;
        public override BaseAttackVariation AttackVariation => Scriptable.AttackVariation;
        public override DiceSet AttackDiceSet => Scriptable.OverrideAttackDice(out DiceSet dice) ? dice : FieldManager.MonsterAttackDice;
        public override DiceSet MagicDiceSet => Scriptable.OverrideMagicDice(out DiceSet dice) ? dice : FieldManager.MonsterMagicDice;
        public override DiceSet DefenseDiceSet => Scriptable.OverrideDefenseDice(out DiceSet dice) ? dice : FieldManager.MonsterDefenseDice;
        protected override float SharedLootDropModifier => Scriptable.SharedLootDropModifier;
        public override bool CanAct => Scriptable.AllowedToAct && base.CanAct;
        public override bool CanAttack => Scriptable.AllowedToAttack && base.CanAttack;
        public override bool CanWalk => Scriptable.AllowedToMove && base.CanWalk;
        public override bool CanCast => Scriptable.AllowedToCast && base.CanCast;
    }
}