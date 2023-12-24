using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Terrain Effect")]
    public class TerrainEffect : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private Sprite icon;

        public string Title => title;
        public Sprite Icon => icon;
    }
}