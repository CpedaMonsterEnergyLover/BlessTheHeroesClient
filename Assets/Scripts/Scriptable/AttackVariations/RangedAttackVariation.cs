using UnityEngine;

namespace Scriptable.AttackVariations
{
    [CreateAssetMenu(menuName = "Attack Variation/Ranged")]
    public class RangedAttackVariation : BaseAttackVariation
    {
        [SerializeField] private Color trailColor;

        public Color TrailColor => trailColor;
    }
}