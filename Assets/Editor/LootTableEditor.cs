using UnityEditor;
using UnityEngine;
using Util.LootTables;

namespace Editor
{
    [CustomEditor (typeof (LootTable), true), CanEditMultipleObjects]
    public class LootTableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            LootTable manager = target as LootTable;
            if (manager == null) return;
            
            DrawDefaultInspector();
            
            if(GUILayout.Button("Add Element"))
                manager.AddItem();
            
            if (GUILayout.Button("Flatten")) 
                manager.Flatten();
        }
    }
}