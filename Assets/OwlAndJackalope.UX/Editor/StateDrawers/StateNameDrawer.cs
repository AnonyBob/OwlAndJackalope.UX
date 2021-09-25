using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.StateDrawers
{
    [CustomPropertyDrawer(typeof(StateObserver))]
    public class StateNameDrawer : PropertyDrawer
    {
        public class PropertyData
        {
            public StateModule Module;
        }
        private readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var name = property.FindPropertyRelative("StateName");
            if (!_data.TryGetValue(property.propertyPath, out var propertyData))
            {
                propertyData = new PropertyData();
            }

            if (propertyData.Module == null)
            {
                var containingObject = (property.serializedObject.targetObject as MonoBehaviour);
                if (containingObject != null) //TODO: Check if there is a module set instead of assuming.
                {
                    propertyData.Module = containingObject.GetComponentInParent<StateModule>();
                }
            }

            if (propertyData.Module != null)
            {
                var options = GetOptions(propertyData);
                var currentIndex = Math.Max(0, Array.IndexOf(options, name.stringValue));
                var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options);
                if (selectedIndex >= 0 && selectedIndex < options.Length)
                {
                    name.stringValue = options[selectedIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, name, label);
            }
        }

        private string[] GetOptions(PropertyData propertyData)
        {
            return propertyData.Module.GetStateNames().ToArray();
        }
    }
}