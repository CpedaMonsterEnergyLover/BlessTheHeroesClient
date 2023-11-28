using System.Linq;
using Gameplay.GameField;
using UnityEngine;

namespace Gameplay.Aggro
{
    public class AggroCollector : MonoBehaviour
    {
        [SerializeField] private Card card;

        private float collectedAggro;
        private int collectedFrame;

        public float GetAggro()
        {
            if (Time.frameCount != collectedFrame)
            {
                collectedFrame = Time.frameCount;
                collectedAggro = card.Heroes.Sum(h => h.AggroManager.AggroLevel);
            }
            
            return collectedAggro;
        }
    }
}