using System.Collections.Generic;
using UnityEngine;
using Util.Cards;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Location/Floor")]
    public class Floor : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private List<Location> storyCards = new();
        [SerializeField] private List<Location> allCards = new();

        public string Name => name;
        public int StoryLength => storyCards.Count;

        
        
        public FloorCardData GetCardData() => new(allCards);
        public Location GetStoryCard(int index) => storyCards[index];
    }
}