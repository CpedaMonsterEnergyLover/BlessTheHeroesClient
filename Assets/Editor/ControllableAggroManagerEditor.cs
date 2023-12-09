using Gameplay.Aggro;

namespace Editor
{
    using UnityEditor;
    using UnityEngine;

    namespace Editor
    {
        [CustomEditor (typeof (ControllableAggroManager), true), CanEditMultipleObjects]
        public class ControllableAggroManagerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI() {
                if (target is not ControllableAggroManager manager) return;
            
                DrawDefaultInspector();
            
                if(GUILayout.Button("Print Aggro"))
                    manager.Print();
            }
        }
    }
}