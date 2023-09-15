using System.Text;
using CardAPI;
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
        [SerializeReference] private CardAction cardAction;
        [SerializeReference] private EvaluatorSet evaluatorSet;

        
        public string DetailDescription
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (evaluatorSet is not null)
                {
                    sb.Append("Throw an <b>Event</b> dice when opening this card:\n")
                        .Append(evaluatorSet.Description);
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
        public EvaluatorSet EvaluatorSet
        {
            get => evaluatorSet;
            set => evaluatorSet = value;
        }

        public CardAction CardAction
        {
            get => cardAction;
            set => cardAction = value;
        }
    }
}