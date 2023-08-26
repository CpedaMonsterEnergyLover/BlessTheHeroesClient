using System.Collections.Generic;
using System.Linq;
using Scriptable;
using UnityEngine;
using Util.Enums;

namespace Util.Cards
{
    public class FloorCardData
    {
        private readonly Dictionary<int, List<Location>> cardData;
        private readonly int[] sizes;
        

        public FloorCardData(IReadOnlyCollection<Location> allCards)
        {
            int rarities = CardUtility.GetRaritiesAmount();
            cardData = new Dictionary<int, List<Location>>();
            for(int i = 0; i < rarities; i++) 
                cardData.Add(i, new List<Location>());
            sizes = new int[rarities];
            
            for (int i = 0; i < rarities; i++)
            {
                var rarity = (LocationRarity) i;
                var cardsOfRarity = allCards.Where(card => card.Rarity == rarity).ToArray();
                sizes[i] = cardsOfRarity.Length;
                cardData[i].AddRange(cardsOfRarity);
            }
        }

        public bool GetRandomCardOfRarity(int rarity, out Location location)
        {
            location = null;
            int size = sizes[rarity];
            if (size == 0) return false;
            location = cardData[rarity][Random.Range(0, size)];
            if (location.Unique)
            {
                sizes[rarity]--;
                cardData[rarity].Remove(location);
            }
            return true;
        }
        
        public Location GetRandomCardClosestToRarity(int rarity)
        {
            rarity = Mathf.Clamp(rarity, 0, CardUtility.GetRaritiesAmount());
            for (int i = rarity; i > 0; i--)
                if (GetRandomCardOfRarity(rarity, out Location card))
                    return card;

            return cardData[0][Random.Range(0, sizes[0])];
        }
    }
}