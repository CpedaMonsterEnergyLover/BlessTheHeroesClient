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
        [SerializeField] private BuffEffectDirection effectDirection;
        [SerializeField] private bool dispellable;
        [SerializeField, TextArea] private string description;
        
        public string Name => name;
        public Sprite Icon => icon;
        public BuffEffectDirection EffectDirection => effectDirection;
        public BuffEffectType EffectType => effectType;
        public bool Dispellable => dispellable;
        public string Description => description;
    }
}