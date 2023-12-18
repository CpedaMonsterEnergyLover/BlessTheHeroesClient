using UnityEngine;

namespace UI.Tooltips
{
    public class TooltipManager : MonoBehaviour
    {
        private static TooltipManager Instance { get; set; }
        
        [SerializeField] private InventoryTooltip inventoryTooltip;
        [SerializeField] private EquipmentTooltip equipmentTooltip;
        [SerializeField] private AbilityTooltip abilityTooltip;

        public static InventoryTooltip InventoryTooltip => Instance.inventoryTooltip;
        public static EquipmentTooltip EquipmentTooltip => Instance.equipmentTooltip;
        public static AbilityTooltip AbilityTooltip => Instance.abilityTooltip;
        
        

        private TooltipManager() => Instance = this;
    }
}