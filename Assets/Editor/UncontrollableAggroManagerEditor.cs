using Gameplay.Aggro;

namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    namespace Editor
    {
        [CustomEditor (typeof (UncontrollableAggroManager), true), CanEditMultipleObjects]
        public class UncontrollableAggroManagerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI() {
                if (target is not UncontrollableAggroManager manager) return;
            
                DrawDefaultInspector();
            
                if(GUILayout.Button("Print Aggro"))
                    manager.Print();
            }
        }
    }
}