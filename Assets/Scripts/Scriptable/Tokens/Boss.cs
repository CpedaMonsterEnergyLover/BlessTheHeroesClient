using UnityEngine;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Boss")]
    public class Boss : Token
    {
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField, Range(1, 3)] private int defenseDiceAmount;
        [SerializeField] private DiceSet attackDice;
        [SerializeField] private DiceSet magicDice;
        [SerializeField] private DiceSet defenseDice;
        [SerializeField] private DropTable dropTable = new();

        
        public int AttackDiceAmount => attackDiceAmount;
        public int DefenseDiceAmount => defenseDiceAmount;
        public DiceSet AttackDice => attackDice;
        public DiceSet MagicDice => magicDice;
        public DiceSet DefenseDice => defenseDice;
        public override DropTable DropTable => dropTable;
    }
}