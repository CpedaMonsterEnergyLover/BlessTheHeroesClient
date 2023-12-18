using Cysharp.Threading.Tasks;
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

            if (GUILayout.Button("Add item"))
                manager.AddItem(manager.debug_ItemToAdd, manager.debug_ToAddAmount, out int leftAmount);
            if (GUILayout.Button("Clear"))
                manager.Clear();
        }
    }
}