using System.Text;
using UnityEngine;
using Util.Enums;
using Util.Dice;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Dice Sets/Normal")]
    public class DiceSet : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private DiceType diceType;
        [SerializeField] private Gameplay.Dice.Dice prefab;
        [SerializeField] private DiceSidesData[] dices = { new(), new(), new() };

        public virtual int DiceAmount => 3;

        public string Name => name;
        public DiceType DiceType => diceType;
        public Gameplay.Dice.Dice Prefab => prefab;

        

        public int DistributedBonus { get; set; }
        public int[] GetDiceValues(int index)
        {
            bool zeroIsMiss = diceType is DiceType.Attack;
            var values = dices[index].Values;
            int whole = DistributedBonus / 18;
            int left = DistributedBonus % 18;
            for (var i = 0; i < values.Length; i++)
            {
                if(values[i] == 0 && zeroIsMiss) continue;
                
                int globalIndex = index * 6 + i;
                values[i] += whole + (globalIndex < left ? 1 : 0);
            }
            return values;
        }

        public int[] GetDiceEnergy(int index) => dices[index].Energy;
        
        public string[] GetDiceSideStrings(int index)
        {
            string[] sides = new string[6];
            int[] values = GetDiceValues(index);
            int[] energy = GetDiceEnergy(index);
            StringBuilder sb = new();
            for (var side = 0; side < 6; side++)
            {
                int value = values[side];
                if (value == 0) sb.Append(".");
                else
                {
                    sb.Append(value).Append("\n");
                    for (int e = 0; e < energy[side]; e++) sb.Append("*");
                }

                sides[side] = sb.ToString();
                sb.Clear();
            }

            return sides;
        }
    }
}