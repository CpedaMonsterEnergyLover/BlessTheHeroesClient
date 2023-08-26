using UnityEngine;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Creature")]
    public class Creature : Token
    {
        [Header("Creature fields")] 
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField, Range(1, 3)] private int defenseDiceAmount;
        [SerializeField] private DiceSet overrideAttackDice;
        [SerializeField] private DiceSet overrideMagicDice;
        [SerializeField] private DiceSet overrideDefenseDice;
        [SerializeField] private DropTable dropTable = new();
        
        public int AttackDiceAmount => attackDiceAmount;
        public int DefenseDiceAmount => defenseDiceAmount;
        public DropTable DropTable => dropTable;
        
        
        public bool OverrideAttackDice(out DiceSet overrideSet)
        {
            overrideSet = overrideAttackDice;
            return overrideSet is not null;
        }
        
        public bool OverrideMagicDice(out DiceSet overrideSet)
        {
            overrideSet = overrideMagicDice;
            return overrideSet is not null;
        }
        
        public bool OverrideDefenseDice(out DiceSet overrideSet)
        {
            overrideSet = overrideDefenseDice;
            return overrideSet is not null;
        }
    }
}