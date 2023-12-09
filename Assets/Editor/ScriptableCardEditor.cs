using Scriptable;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (Location), true)]
    public class ScriptableCardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            Location location = target as Location;
            if (location == null) return;
            
            DrawDefaultInspector();
            
            if (location.Sprite == null) return;

            Texture2D texture = AssetPreview.GetAssetPreview(location.Sprite);
            GUILayout.Label("", GUILayout.Height(300), GUILayout.Width(300));
            
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }
    }
}