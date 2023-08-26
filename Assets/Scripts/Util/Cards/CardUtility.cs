using System;
using Util.Enums;

namespace Util.Cards
{
    public static class CardUtility
    {
        public static int GetRaritiesAmount() => Enum.GetValues(typeof(LocationRarity)).Length - 1;
    }
}