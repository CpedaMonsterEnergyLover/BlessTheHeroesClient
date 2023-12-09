using System.Linq;
using Gameplay.Cards;
using UnityEngine;

namespace Gameplay.GameField
{
    public class FieldGrid : MonoBehaviour
    {
        [SerializeField] private Vector2 cellSize;


        
        public Vector3 GetPointInWorld(int x, int y) => new(x * cellSize.x, 0, y * cellSize.y);

        public void Attach(Card card, int x, int y)
        {
#if UNITY_EDITOR
            card.gameObject.name = $"Card_{x}_{y}";
#endif
            float localX = x * cellSize.x;
            float localY = y * cellSize.y;
            Transform t = card.transform;
            t.SetParent(transform);
            t.localPosition = new Vector3(localX, 0, localY);
        }

        public void Clear()
        {
            var children = transform.Cast<Transform>().ToList();

            foreach (Transform t in children)
            {
#if UNITY_EDITOR
                if(Application.isPlaying) Destroy(t.gameObject);
                else DestroyImmediate(t.gameObject);
#else
                Destroy(t.gameObject)
#endif 
            }
        }
        

    }
    
    
}
