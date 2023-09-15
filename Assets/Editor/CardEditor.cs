using Gameplay.GameField;
using Gameplay.Tokens;
using Scriptable;
using UnityEditor;
using UnityEngine;
using Util;

namespace Editor
{
    [CustomEditor (typeof (Card), true), CanEditMultipleObjects]
    public class CardEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            Card manager = target as Card;
            if (manager == null) return;
            
            DrawDefaultInspector();

            if (GUILayout.Button("Spawn Creature"))
            {
                IToken token = manager.debug_TokenToSpawn switch
                {
                    Boss boss => GlobalDefinitions.CreateBossToken(boss),
                    Creature creature => GlobalDefinitions.CreateCreatureToken(creature),
                    Hero hero => GlobalDefinitions.CreateHeroToken(hero),
                    _ => null
                };
                
                if(token is null) return;

                token.SetCard(manager);
                manager.AddToken(token);
            }
        }
    }
}