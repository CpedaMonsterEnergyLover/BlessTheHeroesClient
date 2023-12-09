using MyBox;
using UnityEngine;

namespace Scriptable.AttackVariations
{
    [CreateAssetMenu(menuName = "Attack Variation/Magic")]
    public class MagicAttackVariation : BaseAttackVariation
    {
        [Separator("Trail")] 
        [SerializeField] private Gradient trailGradient;
        
        [Separator("Castball")] 
        [SerializeField] private bool hasCastball;
        [SerializeField, ConditionalField(nameof(hasCastball), false, true)] 
        private Gradient castballGradient;
        
        [Separator("Sparks")] 
        [SerializeField] private bool hasSparks;
        [SerializeField, ConditionalField(nameof(hasSparks), false, true)] 
        private Gradient sparksGradient;
        
        [Separator("Impact")] 
        [SerializeField] private bool hasImpact;
        [SerializeField, ConditionalField(nameof(hasImpact), false, true)] 
        private Gradient impactGradient;
        
        [Separator("Light")] 
        [SerializeField] private Color lightColor;

        public Gradient TrailGradient => trailGradient;
        public bool HasCastball => hasCastball;
        public Gradient CastballGradient => castballGradient;
        public bool HasSparks => hasSparks;
        public Gradient SparksGradient => sparksGradient;
        public bool HasImpact => hasImpact;
        public Gradient ImpactGradient => impactGradient;
        public Color LightColor => lightColor;
    }
}