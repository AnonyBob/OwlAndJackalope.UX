using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    public class StateModuleEditor
    {
        private const string StateName = "_name";
        private const string Conditions = "_conditions";
        
        private readonly SerializedProperty _moduleProperty;
        private readonly SerializedProperty _statesProperty;
        private readonly SerializedObject _serializedObject;

        private SerializedProperty _selectedState;
        private int? _selectedStateIndex;
        
        public StateModuleEditor(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _moduleProperty = _serializedObject.FindProperty("_stateModule");
            _statesProperty = _moduleProperty.FindPropertyRelative("_states");
        }
        
        public void Draw()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var selectedStateIndex = EditorGUILayout.Popup("States", _selectedStateIndex ?? 0, GetStateNames().ToArray());
                if (!_selectedStateIndex.HasValue || selectedStateIndex != _selectedStateIndex.Value && _statesProperty.arraySize > 0)
                {
                    _selectedStateIndex = selectedStateIndex;
                    _selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);
                }

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    AddState();
                }
            }

            if (_selectedState != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var stateName = _selectedState.FindPropertyRelative(StateName);
                    var newStateName = EditorGUILayout.TextField("State Name", stateName.stringValue);
                    if (newStateName != stateName.stringValue)
                    {
                        stateName.stringValue = HandleNameChange(newStateName, stateName.stringValue);
                    }

                    if (GUILayout.Button("Delete", GUILayout.Width(100)))
                    {
                        DeleteSelectedState();
                    }
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_selectedState.FindPropertyRelative(Conditions), true);
                EditorGUI.indentLevel--;
            }
            
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
            _selectedState = _statesProperty.GetArrayElementAtIndex(_selectedStateIndex.Value);

            _selectedState.FindPropertyRelative(StateName).stringValue = $"State{Guid.NewGuid().ToString().Substring(0, 6)}";
            _selectedState.FindPropertyRelative(Conditions).ClearArray();
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

            if(_serializedObject.targetObject is IStateNameChangeHandler handler)
            {
                handler.HandleStateNameChange(oldName, newName);
            }
            
            return newName;
        }

        private void DeleteSelectedState()
        {
            _statesProperty.DeleteArrayElementAtIndex(_selectedStateIndex.Value);
            _selectedStateIndex = null;
            _selectedState = null;
        }
    }
}