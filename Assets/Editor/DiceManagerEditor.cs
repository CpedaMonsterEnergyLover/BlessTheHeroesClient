using Gameplay.Dice;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor (typeof (DiceManager), true), CanEditMultipleObjects]
    public class DiceManagerEditor : UnityEditor.Editor
    {
        public override async void OnInspectorGUI() {
            DiceManager manager = target as DiceManager;
            if (manager == null) return;
            
            DrawDefaultInspector();

            // if (GUILayout.Button("Throw"))
            //  DiceManager.Throw(manager.debug_AmountToThrow, DiceType.Attack);
            
            if (GUILayout.Button("ThrowReplay"))
            {
                await DiceManager.ThrowReplay(
                    manager.debug_DiceToThrow,
                    manager.debug_AmountToThrow, 
                    manager.debug_SidesToThrow);
            }
        }
    }
}