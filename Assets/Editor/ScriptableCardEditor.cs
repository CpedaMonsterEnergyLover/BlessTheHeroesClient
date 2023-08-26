using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util.Dice;

namespace Editor
{
    [CustomEditor (typeof (Scriptable.Location), true), CanEditMultipleObjects]
    public class ScriptableCardEditor : UnityEditor.Editor
    {
        private SerializedProperty nameProperty;
        private SerializedProperty rarityProperty;
        private SerializedProperty uniqueProperty;
        private SerializedProperty spriteProperty;
        private SerializedProperty descriptionProperty;
        private SerializedProperty droptTableProperty;
        
        
        private int selectedActionIndex;
        private List<Type> actionTypes;
        private SerializedProperty actionProperty;

        private int selectedEvaluatorIndex;
        private List<Type> evaluatorTypes;
        private SerializedProperty evaluatorProperty;


        private void OnEnable()
        {
            actionProperty = serializedObject.FindProperty("cardAction");
            evaluatorProperty = serializedObject.FindProperty("evaluatorSet");
            
            nameProperty = serializedObject.FindProperty("name");
            rarityProperty = serializedObject.FindProperty("rarity");
            uniqueProperty = serializedObject.FindProperty("unique");
            spriteProperty = serializedObject.FindProperty("sprite");
            descriptionProperty = serializedObject.FindProperty("description");
            droptTableProperty = serializedObject.FindProperty("dropTable");
        }


        public override void OnInspectorGUI() {
            Scriptable.Location manager = target as Scriptable.Location;
            if (manager == null) return;
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(nameProperty);
            EditorGUILayout.PropertyField(rarityProperty);
            EditorGUILayout.PropertyField(uniqueProperty);
            EditorGUILayout.PropertyField(spriteProperty);
            EditorGUILayout.PropertyField(descriptionProperty);
            EditorGUILayout.PropertyField(droptTableProperty);


            EditorGUILayout.Separator();
            GUI.color = Color.yellow;
            GUILayout.Label("========= Action settings =========");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(actionProperty, includeChildren: true);

            // Actions
            if (actionTypes is null)
            {
                if (GUILayout.Button("Change Action"))
                {
                    UpdateActionTypes();
                }
            }
            else
            {
                var names = new List<string> {"None"};
                names.AddRange(actionTypes.Select(t => t.Name));
                selectedActionIndex = EditorGUILayout.Popup("Select Action", selectedActionIndex, names.ToArray());
                
                if (GUILayout.Button("Set action"))
                {
                    if (selectedActionIndex == 0)
                    {
                        manager.CardAction = null;
                    } else {
                        var T = actionTypes[selectedActionIndex - 1];
                        manager.CardAction = (CardAPI.CardAction) Activator.CreateInstance(T);
                    }
                }
            }
            
            EditorGUILayout.Separator();
            GUI.color = Color.yellow;
            GUILayout.Label("======== Evaluator settings ========");
            GUI.color = Color.white;
            EditorGUILayout.PropertyField(evaluatorProperty, includeChildren: true);

            // Evaluators
            if (manager.EvaluatorSet is null)
            {
                if (GUILayout.Button("Create Evaluator"))
                {
                    if(evaluatorTypes is null) UpdateEvaluatorTypes();
                    manager.EvaluatorSet = new EvaluatorSet();
                }
            }
            else 
            {
                if (evaluatorTypes is null || actionTypes is null)
                {
                    if (GUILayout.Button("Change Evaluator"))
                    {
                        if(evaluatorTypes is null) UpdateEvaluatorTypes();
                        if(actionTypes is null) UpdateActionTypes();
                    }
                }
                else
                {
                    EditorGUILayout.Separator();
                    var names = new List<string>();
                    names.AddRange(evaluatorTypes!.Select(t => t.Name));
                    selectedEvaluatorIndex =
                        EditorGUILayout.Popup("Select Evaluator", selectedEvaluatorIndex, names.ToArray());
                    if (GUILayout.Button("Add evaluator"))
                    {
                        manager.EvaluatorSet.AddEvaluator(
                            (UniversalDiceEvaluator) Activator.CreateInstance(evaluatorTypes[selectedEvaluatorIndex]));
                    }
                    EditorGUILayout.Separator();
                    names = new List<string>();
                    names.AddRange(actionTypes!.Select(t => t.Name));
                    selectedActionIndex = EditorGUILayout.Popup("Select Action", selectedActionIndex, names.ToArray());
                    if (GUILayout.Button("Add action"))
                    {
                        manager.EvaluatorSet.AddAction(
                            (CardAPI.CardAction) Activator.CreateInstance(actionTypes[selectedActionIndex]));
                    }
                }
            }

            EditorGUILayout.Separator();
            if (manager.EvaluatorSet is not null && GUILayout.Button("Clear Evaluator"))
            {
                manager.EvaluatorSet = null;
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateActionTypes()
        {
            Type parentType = typeof(CardAPI.CardAction);
            actionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(parentType) && t != parentType)
                .ToList();
        }

        private void UpdateEvaluatorTypes()
        {
            Type parentType = typeof(Util.Dice.UniversalDiceEvaluator);
            evaluatorTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(parentType) && t != parentType)
                .ToList();
        }
    }
}