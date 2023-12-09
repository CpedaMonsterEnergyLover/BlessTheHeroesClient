using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Scriptable.AttackVariations
{
    public abstract class BaseAttackVariation : ScriptableObject
    {
        [SerializeField] private new string name;

        public string Name => name;
    }
}