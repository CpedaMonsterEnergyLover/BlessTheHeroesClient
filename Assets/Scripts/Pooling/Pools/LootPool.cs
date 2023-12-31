﻿using UnityEngine;

namespace Pooling
{
    public class LootPool : ObjectPool<EffectLoot>
    {
        [SerializeField] private RectTransform effectTransform;
        

        
        protected override void OnInstantiated(EffectLoot obj)
        {
            obj.transform.SetParent(effectTransform, false);
            obj.Parent = effectTransform;
        }
    }
}