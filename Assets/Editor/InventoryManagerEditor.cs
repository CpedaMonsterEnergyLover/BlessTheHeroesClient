using Gameplay.Dice;
using Gameplay.Inventory;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (InventoryManager), true), CanEditMultipleObjects]
    public class InventoryManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            InventoryManager manager = target as InventoryManager;
            if (manager == null) return;
            
            DrawDefaultInspector();

            // if (GUILayout.Button("Throw"))
            // manager.Throw(manager.debug_SideToRoll);
            
            if (GUILayout.Button("Add item"))
                manager.AddItem(manager.debug_ItemToAdd, manager.debug_ToAddAmount);
            if (GUILayout.Button("Clear"))
                manager.Clear();
        }
    }
}