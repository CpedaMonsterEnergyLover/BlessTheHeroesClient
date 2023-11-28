using Random = UnityEngine.Random;

namespace Util
{
    public static class DiceUtil
    {
        public static bool CalculateAttackDiceThrow(int diceAmount, Scriptable.DiceSet diceSet, int attackPower, out int result, out int[] sides)
        {
            diceSet.DistributedBonus = attackPower;
            sides = new int[diceAmount];
            result = 0;
            bool hit = true;
            for (int i = 0; i < diceAmount; i++)
            {
                int side = Random.Range(0, 6);
                sides[i] = side;
                int sideValue = diceSet.GetDiceValues(i)[side];
                if (sideValue == 0) hit = false;
                result += sideValue;
            }

            return hit;
        }

        public static bool CaclulateMagicDiceThrow(int diceAmount, Scriptable.DiceSet diceSet, int spellPower, out int result, out int[] sides)
        {
            diceSet.DistributedBonus = spellPower;
            sides = new int[diceAmount];
            result = 0;
            for (int i = 0; i < diceAmount; i++)
            {
                int side = Random.Range(0, 6);
                sides[i] = side;
                int sideValue = diceSet.GetDiceValues(i)[side];
                result += sideValue;
            }

            return result > 0;
        }

        public static bool CalculateDefenseDiceThrow(int diceAmount, Scriptable.DiceSet diceSet, int defense, out int result, out int[] sides)
        {
            diceSet.DistributedBonus = defense;
            sides = new int[diceAmount];
            result = 0;
            for (int i = 0; i < diceAmount; i++)
            {
                int side = Random.Range(0, 6);
                sides[i] = side;
                int sideValue = diceSet.GetDiceValues(i)[side];
                result += sideValue;
            }

            return result > 0;
        }
    }
}