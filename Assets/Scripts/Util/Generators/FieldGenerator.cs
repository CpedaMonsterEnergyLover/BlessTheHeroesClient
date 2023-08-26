using Scriptable;
using Util.Cards;
using Random = UnityEngine.Random;

namespace Util.Generators
{
    public static class FieldGenerator
    {
        public static readonly float[] RarityPercents = {0.5f, 0.35f, 0.15f};
        
        public static FieldData GenerateFloor(Floor floor, int size)
        {
            FieldData fieldData = new FieldData(size);
            FillEmptyCards(fieldData, floor);
            return fieldData;
        }
        
        private static void FillEmptyCards(FieldData fieldData, Floor floor)
        {
            var floorCardData = floor.GetCardData();
            for (int i = 0; i < fieldData.Volume; i++)
            {
                if(!fieldData.IsEmpty(i)) continue;
                fieldData.SetCard(GetRandomCard(floorCardData), i);
            }
        }
        
        private static Location GetRandomCard(FloorCardData floorCardData)
        {
            float value = Random.value;
            float step = 0;
            int rarityCounter = 0;
            int raritiesAmount = CardUtility.GetRaritiesAmount();
            do {
                step += RarityPercents[rarityCounter];
                if (value <= step)
                    return floorCardData.GetRandomCardOfRarity(rarityCounter, out Location card)
                        ? card
                        : floorCardData.GetRandomCardClosestToRarity(rarityCounter);
                rarityCounter++;
            } while (rarityCounter < raritiesAmount);
            
            return floorCardData.GetRandomCardClosestToRarity(raritiesAmount);
        }
    }
}