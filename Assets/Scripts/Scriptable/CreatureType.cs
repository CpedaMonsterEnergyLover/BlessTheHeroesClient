using System.Linq;
using System.Text;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(menuName = "Creature Type")]
    public class CreatureType : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string title;
        [SerializeField] private DamageType[] resistantTo;
        [SerializeField] private DamageType[] vulnerableTo;

        public Sprite Icon => icon;
        public string Title => title;
        public bool ResistantTo(DamageType damageType) => resistantTo.Contains(damageType);
        public bool VulnerableTo(DamageType damageType) => vulnerableTo.Contains(damageType);

        
        
        public void AddResistances(StringBuilder sb)
        {
            int len = resistantTo.Length;
            if (len == 0) return;
            sb.Append("\nTakes 50% less damage from ");
            for (int i = 0; i < len; i++)
            {
                if (i != 0) sb.Append(i == len - 1 ? " and " : ", ");
                sb.Append($"{resistantTo[i].ColoredTitle}");
            }
            sb.Append(" attacks");
        }
        
        public void AddVulnerabilities(StringBuilder sb)
        {
            int len = vulnerableTo.Length;
            if (len == 0) return;
            sb.Append("\nTakes 50% more damage from ");
            for (int i = 0; i < len; i++)
            {
                if (i != 0) sb.Append(i == len - 1 ? " and " : ", ");
                sb.Append($"{vulnerableTo[i].ColoredTitle}");
            }
            sb.Append(" attacks");
        }
        
        private void OnValidate()
        {
            if (resistantTo is null || vulnerableTo is null) return;
            if (resistantTo.ToHashSet().Overlaps(vulnerableTo.ToHashSet()))
                Debug.LogError("Resistances and vulnerabilities cannot share common damage types.");
        }
    }
}