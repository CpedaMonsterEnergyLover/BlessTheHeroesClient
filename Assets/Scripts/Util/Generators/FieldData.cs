using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Util.Generators
{
    public class FieldData
    {
        private readonly Scriptable.Location[] cards;
        public int Size { get; }
        public int Volume { get; }
        public Vector2Int Entrance { get; private set; }

        
        
        public FieldData(int size)
        {
            Size = size;
            Volume = size * size;
            cards = new Scriptable.Location[Volume];
            Entrance = IndexToPoint(GetRandomIndex());
        }
        
        public int GetRandomEmptyIndex()
        {
            while (true)
            {
                int index = GetRandomIndex();
                if (cards[index] is null) return index;
            }
        }

        public void IterateMatrix(Action<int, int, Scriptable.Location> a)
        {
            for (int x = 0; x < Size; x++)
            for (int y = 0; y < Size; y++)
                a(x, y, cards[x * Size + y]);
        }

        public bool IsEmpty(int index) => cards[index] == null;
        public int GetRandomIndex() => Random.Range(0, Volume);
        
        public Vector2Int IndexToPoint(int i) => new(i / Size, i % Size);
        public void SetCard(Scriptable.Location location, int index) => cards[index] = location;
    }
}