using Gameplay.Dice;
using Scriptable;
using Scriptable.AttackVariations;

namespace Gameplay.Tokens
{
    public class CompanionToken : ControllableToken<Creature>
    {
        public override DiceSet AttackDiceSet => Scriptable.OverrideAttackDice(out DiceSet dice) ? dice : DiceManager.AttackDiceSet;
        public override DiceSet MagicDiceSet => Scriptable.OverrideMagicDice(out DiceSet dice) ? dice : DiceManager.MagicDiceSet;
        public override DiceSet DefenseDiceSet => Scriptable.OverrideDefenseDice(out DiceSet dice) ? dice : DiceManager.DefenceDiseSet;
        public override DamageType DamageType => Scriptable.DamageType;
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => (int)Scriptable.ArmorType;
        public override BaseAttackVariation AttackVariation => Scriptable.AttackVariation;
        protected override bool CanInteractWithCards => false;
        public override bool CanClick => true;
        protected override int DefaultActionPoints => Scriptable.AllowedToAct ? 1 : 0;

        public override bool CanAct => Scriptable.AllowedToAct && base.CanAct;
        public override bool CanAttack => Scriptable.AllowedToAttack && base.CanAttack;
        public override bool CanWalk => Scriptable.AllowedToMove && base.CanWalk;
        public override bool CanCast => Scriptable.AllowedToCast && base.CanCast;

        protected override void Die(IToken attacker)
        {
        }
    }
}