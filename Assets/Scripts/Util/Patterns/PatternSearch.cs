using System;
using Gameplay.Cards;
using Gameplay.GameField;
using UnityEngine;

namespace Util.Patterns
{
    public static class PatternSearch
    {
        public static void IterateSingle(Vector2Int center, Action<Vector2Int> a) => a(center);
        public static void IterateSingle(Vector2Int center, Action<Card> a)
        {
            if(FieldManager.GetCard(center, out Card card)) a(card);
        }

        public static void IterateSide(Vector2Int center, Vector2Int side, int radius, Action<Vector2Int> a, bool includeCenter = false)
        {
            for (int i = includeCenter ? 0 : 1; i <= radius; i++)
                a(center + side * i);
        }
        
        public static void IterateSide(Vector2Int center, Vector2Int side, int radius, Action<Card> a, bool includeCenter = false)
        {
            for (int i = includeCenter ? 0 : 1; i <= radius; i++)
                if (FieldManager.GetCard(center + side * i, out Card card))
                    a(card);
        }

        public static bool CheckSide(Vector2Int center, Vector2Int target, Vector2Int side, int radius, bool includeCenter = false)
        {
            bool check = false;
            IterateSide(center, side, radius, v => { if (v == target) check = true; }, includeCenter);
            return check;
        }
        
        public static void IterateHorizontal(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(-1, 0), radius, a);
            IterateSide(center, new Vector2Int(1, 0), radius, a);
        }   
        
        public static void IterateHorizontal(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(-1, 0), radius, a);
            IterateSide(center, new Vector2Int(1, 0), radius, a);
        }

        public static bool CheckHorizontal(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            if (includeCenter && center == target) return true;
            bool check = false;

            void Action(Vector2Int v) { if (v == target) check = true; }

            IterateSide(center, new Vector2Int(-1, 0), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(1, 0), radius, Action);
            return check;
        }

        public static void IterateVertical(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(0, -1), radius, a);
            IterateSide(center, new Vector2Int(0, 1), radius, a);
        }
        
        public static void IterateVertical(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(0, -1), radius, a);
            IterateSide(center, new Vector2Int(0, 1), radius, a);
        }

        public static bool CheckVertical(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            if (includeCenter && center == target) return true;
            bool check = false;

            void Action(Vector2Int v) { if (v == target) check = true; }

            IterateSide(center, new Vector2Int(0, -1), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(0, 1), radius, Action);
            return check;
        }

        public static void IterateCrest(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(-1, 1), radius, a);
            IterateSide(center, new Vector2Int(1, 1), radius, a);
            IterateSide(center, new Vector2Int(1, -1), radius, a);
            IterateSide(center, new Vector2Int(-1, -1), radius, a);
        }
        
        public static void IterateCrest(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            IterateSide(center, new Vector2Int(-1, 1), radius, a);
            IterateSide(center, new Vector2Int(1, 1), radius, a);
            IterateSide(center, new Vector2Int(1, -1), radius, a);
            IterateSide(center, new Vector2Int(-1, -1), radius, a);
        }

        public static bool CheckCrest(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            if (includeCenter && center == target) return true;
            bool check = false;

            void Action(Vector2Int v) { if (v == target) check = true; }

            IterateSide(center, new Vector2Int(-1, 1), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(1, 1), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(1, -1), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(-1, -1), radius, Action);
            return check;
        }

        public static void IteratePlus(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            IterateHorizontal(center, radius, a, includeCenter);
            IterateVertical(center, radius, a, false);
        }
        
        public static void IteratePlus(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            IterateHorizontal(center, radius, a, includeCenter);
            IterateVertical(center, radius, a, false);
        }
        
        public static bool CheckPlus(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            if (includeCenter && center == target) return true;
            bool check = false;

            void Action(Vector2Int v) { if (v == target) check = true; }

            IterateSide(center, new Vector2Int(-1, 0), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(1, 0), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(0, -1), radius, Action);
            if (check) return true;
            IterateSide(center, new Vector2Int(0, 1), radius, Action);
            return check;
        }

        public static void IterateNeighbours(Vector2Int center, Action<Vector2Int> a)
        {
            IterateSingle(center + new Vector2Int(-1, 0), a);
            IterateSingle(center + new Vector2Int(1, 0), a);
            IterateSingle(center + new Vector2Int(0, -1), a);
            IterateSingle(center + new Vector2Int(0, 1), a);
        }
        
        public static void IterateNeighbours(Vector2Int center, Action<Card> a)
        {
            IterateSingle(center + new Vector2Int(-1, 0), a);
            IterateSingle(center + new Vector2Int(1, 0), a);
            IterateSingle(center + new Vector2Int(0, -1), a);
            IterateSingle(center + new Vector2Int(0, 1), a);
        }
        
        public static bool CheckNeighbours(Vector2Int center, Vector2Int target) => CheckPlus(center, target, 1, false);

        public static void IterateStar(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            IterateCrest(center, radius, a, includeCenter);
            IteratePlus(center, radius, a, false);
        }
        
        public static void IterateStar(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            IterateCrest(center, radius, a, includeCenter);
            IteratePlus(center, radius, a, false);
        }

        public static bool CheckStar(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            bool check = false;

            void Action(Vector2Int v) { if (v == target) check = true; }
            IterateCrest(center, radius, Action, includeCenter);
            if (check) return true;
            IteratePlus(center, radius, Action, false);
            return check;
        }

        public static void IterateArea(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            for(int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int point = new Vector2Int(x, y);
                if(point.Equals(Vector2Int.zero)) continue;
                a(center + point);
            }
        }
        
        public static void IterateArea(Vector2Int center, int radius, Action<Card> a, bool includeCenter = true)
        {
            if(includeCenter) IterateSingle(center, a);
            for(int x = -radius; x <= radius; x++)
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int point = new Vector2Int(x, y);
                if(point.Equals(Vector2Int.zero)) continue;
                if(FieldManager.GetCard(center + point, out Card card)) a(card);
            }
        }

        public static bool CheckArea(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            bool check = false;
            void Action(Vector2Int v) { if (v == target) check = true; }
            IterateArea(center, radius, Action, includeCenter);
            return check;
        }

        public static void IteratePattern(
            Pattern pattern,
            Vector2Int center,
            Action<Vector2Int> a,
            int radius = 1,
            Vector2Int side = default,
            bool includeCenter = false)
        {
            switch (pattern)
            {
                case Pattern.Single:
                    IterateSingle(center, a);
                    break;
                case Pattern.Side:
                    if (side == default) side = Vector2Int.zero;
                    IterateSide(center, side, radius, a, includeCenter);
                    break;
                case Pattern.Horizontal:
                    IterateHorizontal(center, radius, a, includeCenter);
                    break;
                case Pattern.Vertical:
                    IterateVertical(center, radius, a, includeCenter);
                    break;
                case Pattern.Crest:
                    IterateCrest(center, radius, a, includeCenter);
                    break;
                case Pattern.Plus:
                    IteratePlus(center, radius, a, includeCenter);
                    break;
                case Pattern.Neighbours:
                    IterateNeighbours(center, a);
                    break;
                case Pattern.Star:
                    IterateStar(center, radius, a, includeCenter);
                    break;
                case Pattern.Area:
                    IterateArea(center, radius, a, includeCenter);
                    break;
            }
        }

        public static void IteratePattern(
            Pattern pattern,
            Vector2Int center,
            Action<Card> a,
            int radius = 1,
            Vector2Int side = default,
            bool includeCenter = false)
        {
            IteratePattern(
                pattern,
                center,
                v =>
                {
                    if (FieldManager.GetCard(v, out Card c)) a(c);
                },
                radius,
                side,
                includeCenter);
        }

    }
}