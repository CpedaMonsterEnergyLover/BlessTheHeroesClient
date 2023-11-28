using Scriptable;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (Creature), true), CanEditMultipleObjects]
    public class ScriptableCreatureEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            Creature creature = target as Creature;
            if (creature == null) return;
            
            DrawDefaultInspector();
            
            if (creature.Sprite == null) return;

            Texture2D texture = AssetPreview.GetAssetPreview(creature.Sprite);
            GUILayout.Label("", GUILayout.Height(100), GUILayout.Width(100));
            if(texture is null) return;
            
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }
    }
}