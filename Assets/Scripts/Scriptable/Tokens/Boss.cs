using MyBox;
using Scriptable.AttackVariations;
using UnityEngine;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Boss")]
    public class Boss : Token
    {
        [SerializeField] private MeleeAttackVariation attackVariation;
        [Separator("Boss fields")] 
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField, Range(1, 3)] private int defenseDiceAmount;
        [SerializeField] private DiceSet attackDice;
        [SerializeField] private DiceSet magicDice;
        [SerializeField] private DiceSet defenseDice;
        [Separator("Drop table")]
        [SerializeField] private DropTable dropTable = new();

        public MeleeAttackVariation AttackVariation => attackVariation;
        public int AttackDiceAmount => attackDiceAmount;
        public int DefenseDiceAmount => defenseDiceAmount;
        public DiceSet AttackDice => attackDice;
        public DiceSet MagicDice => magicDice;
        public DiceSet DefenseDice => defenseDice;
        public override DropTable DropTable => dropTable;
    }
}