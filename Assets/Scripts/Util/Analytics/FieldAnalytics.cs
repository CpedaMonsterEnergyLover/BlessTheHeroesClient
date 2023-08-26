using System.Text;
using UnityEngine;
using Util.Cards;
using Util.Enums;
using Util.Generators;

namespace Util.Analytics
{
    public static class FieldAnalytics
    {
        public static void PrintFieldStatistic(FieldData fieldData)
        {
            int rarities = CardUtility.GetRaritiesAmount();
            StringBuilder sb = new StringBuilder().Append("=== Field analytics ===\n");
            sb.Append($"* Field volume: {fieldData.Volume}\n");
            string[] colors = {
                "white",
                "cyan",
                "magenta",
                "lightblue",
                "orange",
                "red"
            };
            

            int[] amounts = new int[rarities];
            int generatedVolume = fieldData.Volume;
            fieldData.IterateMatrix((_, _, card) =>
            {
                if (card.Rarity != LocationRarity.Story)
                    amounts[(int) card.Rarity]++;
                else generatedVolume--;
            });
            sb.Append($"* Necessary cards: {fieldData.Volume - generatedVolume}\n");

            
            for (int i = 0; i < rarities; i++)
            {
                float targetPercent = FieldGenerator.RarityPercents[i];
                LocationRarity rarity = (LocationRarity) i;
                int amount = amounts[i];
                float percent = amount / (float) generatedVolume;
                float diff = percent - targetPercent;
                sb.Append($"* <color={colors[i]}>{rarity.ToString()}</color> cards: {amount} ");
                sb.Append($"({percent:0.##}% ");
                sb.Append($" / <color={(diff < 0 ? "red" : "lime")}>{targetPercent:0.#####}%</color>)\n");
            }
            sb.Append("=== Field analytics ===\n");
            Debug.Log(sb.ToString());
        }
    }
}