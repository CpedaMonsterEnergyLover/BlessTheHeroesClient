using MyBox;
using Scriptable.AttackVariations;
using UnityEngine;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Creature")]
    public class Creature : Token
    {
        [Separator("Creature fields")]
        [SerializeField] private DamageType damageType;
        [SerializeField] private BaseAttackVariation attackVariation;
        [SerializeField] private bool canAct;
        [SerializeField, Range(1, 3)] private int attackDiceAmount;
        [SerializeField] private DiceSet overrideAttackDice;
        [SerializeField] private DiceSet overrideMagicDice;
        [SerializeField] private DiceSet overrideDefenseDice;
        [Separator("Drop table")]
        [SerializeField, Range(0, 1)] private float sharedLootDropModifier;
        [SerializeField] private DropTable dropTable = new();
        
        public DamageType DamageType => damageType;
        public bool CanAct => canAct;
        public int AttackDiceAmount => attackDiceAmount;
        public override DropTable DropTable => dropTable;
        public float SharedLootDropModifier => sharedLootDropModifier;
        public BaseAttackVariation AttackVariation => attackVariation;


        
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