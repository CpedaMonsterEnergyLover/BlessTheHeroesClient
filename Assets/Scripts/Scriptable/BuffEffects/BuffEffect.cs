using UnityEngine;
using Util.Enums;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "BuffEffect")]
    public class BuffEffect : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private BuffEffectType effectType;
        [SerializeField, TextArea] private string description;
        
        
        public string Name => name;
        public Sprite Icon => icon;
        public BuffEffectType EffectType => effectType;
        public string Description => description;
    }
}