using Scriptable;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (Item), true), CanEditMultipleObjects]
    public class ScriptableItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            Item item = target as Item;
            if (item == null) return;
            
            DrawDefaultInspector();
            
            if (item.Sprite == null)
                return;

            Texture2D texture = AssetPreview.GetAssetPreview(item.Sprite);
            GUILayout.Label("", GUILayout.Height(100), GUILayout.Width(100));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        }
    }
}