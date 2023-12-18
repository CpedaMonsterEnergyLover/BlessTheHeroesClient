using UnityEngine;

namespace Scriptable.AttackVariations
{
    public abstract class BaseAttackVariation : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private int damageDelay = 200;

        public string Name => name;
        public int DamageDelay => damageDelay;
    }
}