using Gameplay.Abilities;
using UnityEngine;

namespace Scriptable
{
    public abstract class Token : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int health;
        [SerializeField] private int mana;
        [SerializeField] private Ability[] abilities = new Ability[4];


        public string Name => name;
        public Sprite Sprite => sprite;
        public int Health => health;
        public int Mana => mana;
        public Ability[] Abilities => abilities;
    }
}