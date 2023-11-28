using UnityEngine;

namespace Effects
{
    public class LootPool : EffectsPool<EffectLoot>
    {
        [SerializeField] private RectTransform effectTransform;
        

        
        protected override void OnInstantiated(EffectLoot obj)
        {
            obj.transform.SetParent(effectTransform, false);
            obj.Parent = effectTransform;
        }
    }
}