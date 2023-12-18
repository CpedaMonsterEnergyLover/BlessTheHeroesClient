using MyBox;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Damage Type")]
    public class DamageType : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private DamageOrigin origin;
        [SerializeField] private Color mainColor;
        [SerializeField] private Color secondaryColor;

        public DamageOrigin Origin => origin;
        public string Title => title;
        public string ColoredTitle => $"<color={mainColor.ToHex()}>{title}</color>";
        public Color MainColor => mainColor;
        public Color SecondaryColor => secondaryColor;

        public enum DamageOrigin
        {
            Physic,
            Magic
        }
    }
}