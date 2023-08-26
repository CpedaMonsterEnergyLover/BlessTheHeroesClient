using System;
using UnityEngine;

namespace Util.Patterns
{
    public static class PatternSearch
    {
        public static void IterateSingle(Vector2Int center, Action<Vector2Int> a) => a(center);
        
        public static void IterateSide(Vector2Int center, Vector2Int side, int radius, Action<Vector2Int> a)
        {
            for (int i = 1; i <= radius; i++)
                a(center + side * i);
        }

        public static bool CheckSide(Vector2Int center, Vector2Int target, Vector2Int side, int radius)
        {
            bool check = false;
            IterateSide(center, side, radius, v => { if (v == target) check = true; });
            return check;
        }
        
        public static void IterateHorizontal(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
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
        public static bool CheckNeighbours(Vector2Int center, Vector2Int target) => CheckPlus(center, target, 1, false);

        public static void IterateStar(Vector2Int center, int radius, Action<Vector2Int> a, bool includeCenter = true)
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

        public static bool CheckArea(Vector2Int center, Vector2Int target, int radius, bool includeCenter = true)
        {
            bool check = false;
            void Action(Vector2Int v) { if (v == target) check = true; }
            IterateArea(center, radius, Action, includeCenter);
            return check;
        }
    }
}