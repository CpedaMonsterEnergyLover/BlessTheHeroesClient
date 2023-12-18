using UI.Browsers;
using UnityEngine;

namespace Effects
{
    public class PartyFramePool : ObjectPool<PartyFrame>
    {
        [SerializeField] private RectTransform framesTransform;
        [SerializeField] private UI.Inventory inventory;

        
        protected override void OnInstantiated(PartyFrame obj)
        {
            obj.transform.SetParent(framesTransform, false);
            obj.transform.SetParent(framesTransform);
            obj.Inventory = inventory;
        }
    }
}