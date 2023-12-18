using System.Collections.Generic;
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
        [SerializeField] private CreatureType creatureType;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int health;
        [SerializeField] private int mana;
        [SerializeField] private int speed;
        [SerializeField] private List<Ability> abilities = new();
        [SerializeField] private AttackType attackType;
        [SerializeField] private ArmorType armorType;

        public CreatureType CreatureType => creatureType;
        public abstract DropTable DropTable { get; }
        public string Name => name;
        public Sprite Sprite => sprite;
        public int Health => health;
        public int Mana => mana;
        public AttackType AttackType => attackType;
        public ArmorType ArmorType => armorType;
        public int Speed => speed;
        public List<Ability> Abilities => abilities;
    }

}