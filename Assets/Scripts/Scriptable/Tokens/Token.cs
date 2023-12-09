using Gameplay.Abilities;
using MyBox;
using UnityEngine;
using Util.Enums;
using Util.LootTables;

namespace Scriptable
{
    public abstract class Token : ScriptableObject
    {
        [Separator("Base fields")]
        [SerializeField] private new string name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int health;
        [SerializeField] private int mana;
        [SerializeField] private int speed;
        [SerializeField] private Ability[] abilities = new Ability[4];
        [SerializeField] private AttackType attackType;

        public abstract DropTable DropTable { get; }
        public string Name => name;
        public Sprite Sprite => sprite;
        public int Health => health;
        public int Mana => mana;
        public AttackType AttackType => attackType;
        public int Speed => speed;
        public Ability[] Abilities => abilities;
    }
}