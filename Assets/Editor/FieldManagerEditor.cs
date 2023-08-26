using Gameplay.GameField;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (FieldManager), true), CanEditMultipleObjects]
    public class FieldManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            FieldManager manager = target as FieldManager;
            if (manager == null) return;
            
            DrawDefaultInspector();

            if (GUILayout.Button("Clear")) 
                manager.Clear();

            if (GUILayout.Button("Generate")) 
                manager.GenerateField();

            if (GUILayout.Button("Regenerate"))
            {
                manager.Clear();
                manager.GenerateField();
            }
        }
    }
}

