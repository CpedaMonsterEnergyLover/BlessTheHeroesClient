using System.Collections.Generic;
using Camera;
using Cysharp.Threading.Tasks;
using Gameplay.Tokens;
using UnityEngine;
using Util.Analytics;
using Util.Generators;

namespace Gameplay.GameField
{
    public class FieldManager : MonoBehaviour
    {
        private static FieldManager Instance { get; set; }
        
        [SerializeField] private Scriptable.Anfilade anfilade;
        [SerializeField] private int floorIndex;
        [SerializeField] private int size;
        [SerializeField] private Card cardPrefab;
        [SerializeField] private FieldGrid grid;
        [SerializeField] private List<Scriptable.Hero> heroesToSpawn = new();
        [SerializeField] private AnimationCurve storyProbabilityCurve = new();
        
        private static Scriptable.Floor InstantiatedFloor { get; set; }
        private static int storyProgress;
        private static int fieldSize;
        private static readonly Dictionary<Vector2Int, Card> Cards = new();
        public static Scriptable.DiceSet MonsterAttackDice => Instance.anfilade.MonsterAttackDice;
        public static Scriptable.DiceSet MonsterDefenseDice => Instance.anfilade.MonsterDefenseDice;
        public static Scriptable.DiceSet MonsterMagicDice => Instance.anfilade.MonsterMagicDice;
        public static int OpenedCardsCounter { get; set; }

        
        
        private FieldManager() => Instance = this;
        
        private void Start()
        {
            fieldSize = size;
            Clear();
            GenerateField();
        }

        public void GenerateField()
        {
            if (!anfilade.GetFloor(floorIndex, out var floor))
            {
                Debug.LogError($"Anfilade {anfilade.Name} doesn't have a floor at index {floorIndex}");
                return;
            }

            storyProgress = 0;
            InstantiatedFloor = floor;
            FieldData fieldData = FieldGenerator.GenerateFloor(floor, size);
            fieldData.IterateMatrix(PlaceCard);

            FieldAnalytics.PrintFieldStatistic(fieldData);
            MainCamera.Instance.SetMinPoint(grid.GetPointInWorld(0, 0));
            MainCamera.Instance.SetMaxPoint(grid.GetPointInWorld(size - 1, size - 1));

            var entrance = Cards[fieldData.Entrance];
            OpenEntrance(entrance);
            OpenedCardsCounter = 1;
            entrance.SpawnHeroes(heroesToSpawn).Forget();
        }

        public static bool TryOpenStoryCard(Vector2Int point)
        {
            int storyLength = InstantiatedFloor.StoryLength;
            if (storyProgress >= storyLength) return false;

            float target = 1f / (storyLength * 1.5f);
            float current = Mathf.Clamp01((float) OpenedCardsCounter / (fieldSize * fieldSize) -
                                          storyProgress / (storyLength * 1.5f));
            
            float probability = Mathf.Clamp01(Instance.storyProbabilityCurve.Evaluate(current / target));
            if (Random.value > probability) return false;
            
            Scriptable.Location storyCard = InstantiatedFloor.GetStoryCard(storyProgress);
            storyProgress++;
            Cards[point].SetScriptable(storyCard);
            return false;
        }
        
        private void PlaceCard(int x, int y, Scriptable.Location scriptable)
        {
            var point = new Vector2Int(x, y);
            var card = Instantiate(cardPrefab);
            card.SetScriptable(scriptable);
            card.SetGridPosition(point);
            grid.Attach(card, x, y);
            Cards[point] = card;
        }

        private void OpenEntrance(Card entrance)
        {
            entrance.OpenOnStart = true;
            var pos = entrance.GridPosition;
            MainCamera.Instance.SetPosition(grid.GetPointInWorld(pos.x, pos.y));
        }
        
        public void Clear()
        {
            Cards.Clear();
            grid.Clear();
        }

        public static bool GetCard(Vector2Int position, out Card card)
        {
            card = null;
            if (position.x >= fieldSize || position.y >= fieldSize || position.x < 0 || position.y < 0) return false;
            card = Cards[position];
            return true;
        }

        public static List<IUncontrollableToken> GetAllCreatures()
        {
            List<IUncontrollableToken> creatures = new();
            foreach (var (pos, card) in Cards)
            {
                if(card.IsOpened)
                    creatures.AddRange(card.Creatures);
            }
            return creatures;
        }
    }
}