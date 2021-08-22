using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.StateDrawers
{
    [CustomEditor(typeof(StateModule))]
    public class StateModuleEditor : UnityEditor.Editor
    {
        private const string StateName = "_name";
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
                if (GUILayout.Button("+", GUILayout.Width(30)))
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

                        var originalColor = GUI.contentColor;
                        if (state == null)
                        {
                            GUI.contentColor = Color.yellow;
                            EditorGUILayout.LabelField("⚠️ Not Found", GUILayout.Width(100));
                        }
                        else if(state.IsActive)
                        {
                            GUI.contentColor = Color.green;
                            EditorGUILayout.LabelField("✔️ Active", GUILayout.Width(100));
                        }
                        else
                        {
                            GUI.contentColor = Color.red;
                            EditorGUILayout.LabelField("✖️ Inactive", GUILayout.Width(100));
                        }

                        GUI.contentColor = originalColor;
                    }
                    else
                    {
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            DeleteSelectedState();
                        }
                        GUI.enabled = true;
                    }
                }

                if (_selectedStateIndex.HasValue)
                {
                    var selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);
                    EditorGUILayout.PropertyField(selectedState.FindPropertyRelative(Conditions), true);
                }
            }

            serializedObject.ApplyModifiedProperties();
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
            selectedState.FindPropertyRelative(Conditions).ClearArray();
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
                handler.HandleStateNameChange(oldName, newName);
            }
            
            return newName;
        }

        private void DeleteSelectedState()
        {
            _statesProperty.DeleteArrayElementAtIndex(_selectedStateIndex.Value);
            _selectedStateIndex = null;
        }
    }
}