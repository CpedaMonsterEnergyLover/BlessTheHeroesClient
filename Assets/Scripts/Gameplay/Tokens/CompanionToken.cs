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
        public override int AttackDiceAmount => Scriptable.AttackDiceAmount;
        public override int DefenseDiceAmount => Scriptable.DefenseDiceAmount;
        public override BaseAttackVariation AttackVariation => Scriptable.AttackVariation;
        protected override bool CanInteractWithCards => false;
        public override bool CanClick => true;
        public override bool CanInteract => Scriptable.CanAct && base.CanInteract;

        protected override int DefaultActionPoints => Scriptable.CanAct ? 2 : 0;

        
        
        protected override void Die()
        {
        }
    }
}