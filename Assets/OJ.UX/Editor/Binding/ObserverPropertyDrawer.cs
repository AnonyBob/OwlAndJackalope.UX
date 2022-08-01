using System;
using System.Collections.Generic;
using System.Linq;
using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binding
{
    [CustomPropertyDrawer(typeof(Observer), true)]
    [CustomPropertyDrawer(typeof(ObserveDetailsAttribute), true)]
    public class ObserverPropertyDrawer : PropertyDrawer
    {
        private readonly List<GUIContent> _options = new List<GUIContent>(10);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var referenceModuleProp = property.FindPropertyRelative("_referenceModule");
            var detailProp = property.FindPropertyRelative("_detailName");

            var pos = position;
            pos.width = position.width * 0.7f - 2;
            
            var previousEnabled = GUI.enabled;
            GUI.enabled = !Application.isPlaying;
            
            var options = GetOptions(referenceModuleProp, GetTypes(property));
            if (options.Length == 0 || referenceModuleProp.objectReferenceValue == null)
            {
                EditorGUI.LabelField(pos, "⚠ No valid details available!");
            }
            else
            {
                var isArray = CheckIfInArray();
                var previousSelection = GetPreviousSelection(detailProp, options);
                var newSelection = EditorGUI.Popup(pos, isArray ? GUIContent.none : label, previousSelection, options);
                if (previousSelection != newSelection && newSelection >= 0 && newSelection < options.Length)
                {
                    detailProp.stringValue = options[newSelection].text;
                }
            }

            EditorGUI.BeginChangeCheck();
            
            pos.x = pos.x + pos.width + 2;
            pos.width = position.width - pos.width - 4;
            EditorGUI.PropertyField(pos, referenceModuleProp, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                if (referenceModuleProp.objectReferenceValue == null || options.Length == 0)
                {
                    detailProp.stringValue = null;
                }
            }

            GUI.enabled = previousEnabled;
        }

        private int GetPreviousSelection(SerializedProperty detailProp, GUIContent[] options)
        {
            return Array.FindIndex(options, o => o.text == detailProp.stringValue);
        }

        private Type[] GetTypes(SerializedProperty property)
        {
            var parentProperty = property.FindParentProperty();
            if (parentProperty != null)
            {
                var comparisonProperty = parentProperty.FindPropertyRelative("_comparison");
                if (comparisonProperty != null)
                {
                    if (comparisonProperty.managedReferenceValue is IConditionComparison comparison)
                    {
                        return new Type[] { comparison.GetConditionValueType() };
                    }
                }
            }
            
            if (attribute is ObserveDetailsAttribute observeAttr)
            {
                return observeAttr.AcceptableTypes;
            }

            if (fieldInfo.FieldType.IsGenericType)
            {
                return new Type[] { fieldInfo.FieldType.GetGenericArguments()[0] };
            }
            
            return new Type[0];
        }

        private GUIContent[] GetOptions(SerializedProperty referenceModuleProp, params Type[] types)
        {
            _options.Clear();
            if (referenceModuleProp.objectReferenceValue is ReferenceModule referenceModule)
            {
                var referenceModuleSO = new SerializedObject(referenceModule);
                var details = referenceModuleSO.FindProperty("_serializedReference").FindPropertyRelative("Details");
                for (var i = 0; i < details.arraySize; ++i)
                {
                    var detailProp = details.GetArrayElementAtIndex(i);
                    var detail = detailProp.managedReferenceValue as ISerializedDetail;
                    if (detail != null && types.Any(t => t == detail.GetValueType() 
                                                         || detail.GetValueType().IsSubclassOf(t) 
                                                         || t.IsAssignableFrom(detail.GetValueType())))
                    {
                        _options.Add(new GUIContent(detail.GetName()));
                    }
                }
            }

            return _options.ToArray();
        }

        private bool CheckIfInArray()
        {
            return fieldInfo.FieldType.IsArray || fieldInfo.FieldType == typeof(List<Observer>);
        }
    }
}