using Effects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (ParticleSystemLineRenderer), true), CanEditMultipleObjects]
    public class ParticleSystemLineRendererEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            ParticleSystemLineRenderer manager = target as ParticleSystemLineRenderer;
            if (manager == null) return;
            
            DrawDefaultInspector();

            if (GUILayout.Button("Update mesh")) 
                manager.UpdateMesh();

        }
    }
}