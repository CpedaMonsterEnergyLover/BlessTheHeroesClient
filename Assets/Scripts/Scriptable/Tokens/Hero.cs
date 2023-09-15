using UnityEngine;
using Util.Enums;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Hero")]
    public class Hero : Token
    {
        [SerializeField] private ArmorType armorType;
        [SerializeField] private Equipment[] equipment = new Equipment[4];

        public ArmorType ArmorType => armorType;
        public Equipment[] Equipment => equipment;
        public override DropTable DropTable => null;
    }
}