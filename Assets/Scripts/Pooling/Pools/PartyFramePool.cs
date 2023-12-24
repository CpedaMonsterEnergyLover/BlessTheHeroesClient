using UI.Browsers;
using UnityEngine;

namespace Pooling
{
    public class PartyFramePool : ObjectPool<PartyFrame>
    {
        [SerializeField] private RectTransform framesTransform;

        
        protected override void OnInstantiated(PartyFrame obj)
        {
            obj.transform.SetParent(framesTransform, false);
            obj.transform.SetParent(framesTransform);
        }
    }
}