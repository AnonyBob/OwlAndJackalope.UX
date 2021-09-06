using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.StateDrawers
{
    [CustomEditor(typeof(StateModule))]
    public class StateModuleEditor : UnityEditor.Editor
    {
        private const string StateName = "_name";
        private const string ConditionGroups = "_conditionGroups";
        private const string Conditions = "_conditions";
        
        private SerializedProperty _statesProperty;
        private int? _selectedStateIndex;

        public override void OnInspectorGUI()
        {
            _statesProperty = serializedObject.FindProperty("_states");
            using (new EditorGUILayout.HorizontalScope())
            {
                var selectedStateIndex = EditorGUILayout.Popup("States", _selectedStateIndex ?? 0, GetStateNames().ToArray());
                if ((!_selectedStateIndex.HasValue || selectedStateIndex != _selectedStateIndex.Value) && _statesProperty.arraySize > 0)
                {
                    _selectedStateIndex = selectedStateIndex;
                }

                GUI.enabled = !Application.isPlaying;
                if (SharedDrawers.Button("ADD", Color.green, GUILayout.Width(50)))
                {
                    AddState();
                }

                GUI.enabled = true;
            }

            if (_selectedStateIndex.HasValue && _statesProperty.arraySize > _selectedStateIndex.Value)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = !Application.isPlaying;
                    var selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);
                    var stateName = selectedState.FindPropertyRelative(StateName);
                    var newStateName = EditorGUILayout.TextField("State Name", stateName.stringValue);
                    if (newStateName != stateName.stringValue)
                    {
                        stateName.stringValue = HandleNameChange(newStateName, stateName.stringValue);
                    }

                    if (Application.isPlaying)
                    {
                        GUI.enabled = true;
                        var stateModule = (StateModule) serializedObject.targetObject;
                        var state = stateModule.GetState(stateName.stringValue);
                        
                        if (state == null)
                        {
                            SharedDrawers.Button("⚠️ Not Found", Color.yellow, GUILayout.Width(100));
                        }
                        else if(state.IsActive)
                        {
                            SharedDrawers.Button("✔️ Active", Color.green, GUILayout.Width(100));
                        }
                        else
                        {
                            SharedDrawers.Button("✖️ Inactive", Color.red, GUILayout.Width(100));
                        }
                        
                    }
                    else
                    {
                        if (SharedDrawers.Button("✖",  Color.red, GUILayout.Width(50)))
                        {
                            DeleteSelectedState();
                        }
                        GUI.enabled = true;
                    }
                }

                if (_selectedStateIndex.HasValue)
                {
                    var selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);
                    DrawConditionGroups(selectedState.FindPropertyRelative(ConditionGroups));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionGroups(SerializedProperty conditionGroups)
        {
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.LabelField("Conditions");

            for (var i = 0; i < conditionGroups.arraySize; ++i)
            {
                if(i > 0) 
                    EditorGUILayout.LabelField("OR");

                using (new EditorGUILayout.HorizontalScope("button"))
                {
                    var conditions = conditionGroups.GetArrayElementAtIndex(i).FindPropertyRelative(Conditions);
                    EditorGUILayout.PropertyField(conditions, new GUIContent($"Group {i + 1}"), true);
                    
                    if (SharedDrawers.Button("✖", Color.red, GUILayout.Width(25)))
                    {
                        conditionGroups.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            
            if (SharedDrawers.Button("OR", Color.green, GUILayout.Width(100)))
            {
                conditionGroups.InsertArrayElementAtIndex(conditionGroups.arraySize);
                var group = conditionGroups.GetArrayElementAtIndex(conditionGroups.arraySize - 1);
                group.FindPropertyRelative(Conditions).ClearArray();
            }

            GUI.enabled = true;
        }

        private IEnumerable<string> GetStateNames()
        {
            for (var i = 0; i < _statesProperty.arraySize; ++i)
            {
                yield return _statesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(StateName).stringValue;
            }
        }
        
        private void AddState()
        {
            _selectedStateIndex = _statesProperty.arraySize;
            _statesProperty.InsertArrayElementAtIndex(_statesProperty.arraySize);
            var selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);

            selectedState.FindPropertyRelative(StateName).stringValue = $"State{Guid.NewGuid().ToString().Substring(0, 6)}";
            selectedState.FindPropertyRelative(ConditionGroups).ClearArray();
        }
        
        private void DeleteSelectedState()
        {
            _statesProperty.DeleteArrayElementAtIndex(_selectedStateIndex.Value);
            _selectedStateIndex = null;
        }

        private string HandleNameChange(string newName, string oldName)
        {
            for (var i = 0; i < _statesProperty.arraySize; ++i)
            {
                if (_selectedStateIndex != i)
                {
                    var otherState = _statesProperty.GetArrayElementAtIndex(i).FindPropertyRelative(StateName).stringValue;
                    if (otherState == newName)
                    {
                        return oldName;
                    }
                }
            }

            if(serializedObject.targetObject is IStateNameChangeHandler handler)
            {
                handler.HandleStateNameChange(oldName, newName, null);
            }
            
            return newName;
        }
    }
}