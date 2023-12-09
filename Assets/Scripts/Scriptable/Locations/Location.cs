using System.Text;
using Gameplay.Cards;
using MyBox;
using UnityEngine;
using Util.Dice;
using Util.Enums;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Location/Location card")]
    public class Location : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private LocationRarity rarity;
        [SerializeField] private bool unique;
        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;
        [SerializeField] private DropTable dropTable = new();
        [SerializeField] private bool hasCardAction;
        [SerializeField, ConditionalField(nameof(hasCardAction), false, true)]
        private CardAction cardAction;
        [SerializeField] private bool hasOpeningEvent;
        [SerializeField, ConditionalField(nameof(hasOpeningEvent), false, true)] 
        private EvaluatorSet openingEvaluator = new();


        
        public string DetailDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (hasOpeningEvent)
                {
                    sb.Append("Throw an <b>Event</b> dice when opening this card:\n")
                        .Append(openingEvaluator.Description);
                }
                return sb.ToString();
            }
        }

        public string LiteralDescription => description;
        public Sprite Sprite => sprite;
        public DropTable DropTable => dropTable;
        public bool Unique => unique;
        public string Name => name;
        public LocationRarity Rarity => rarity;
        public bool HasAction => hasCardAction;
        public CardAction CardAction => cardAction;
        public bool HasOpeningEvent => hasOpeningEvent;
        public EvaluatorSet OpeningEvaluator => openingEvaluator;
    }
}