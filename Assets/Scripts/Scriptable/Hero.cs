using Gameplay.Abilities;
using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Token/Hero")]
    public class Hero : Token
    {
        [SerializeField] private int speed;
        [SerializeField] private AttackType attackType;
        [SerializeField] private ArmorType armorType;
        [SerializeField] private Equipment[] equipment = new Equipment[4];

        public int Speed => speed;
        public AttackType AttackType => attackType;
        public ArmorType ArmorType => armorType;
        public Equipment[] Equipment => equipment;
    }
}