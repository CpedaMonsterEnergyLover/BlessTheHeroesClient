using MyBox;
using UnityEngine;
using Util.LootTables;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Hero")]
    public class Hero : Token
    {
        [Separator("Hero fields")]
        [SerializeField] private Equipment[] equipment = new Equipment[4];
        
        
        
        public Equipment[] Equipment => equipment;
        public override DropTable DropTable => null;
    }
}