using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _selectedIndex = -1;
        private readonly List<string> _options = new List<string>(10);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
            
            var labelPos = new Rect(position.x + 5, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelPos, label, EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            
            var posY = labelPos.y + EditorGUIUtility.singleLineHeight;
            var referenceModulePos = new Rect(position.x + 10, posY, position.width - 15, EditorGUIUtility.singleLineHeight);
            var referenceModuleProp = property.FindPropertyRelative("_referenceModule");
            EditorGUI.PropertyField(referenceModulePos, referenceModuleProp, new GUIContent("Reference"));

            posY = referenceModulePos.y + EditorGUIUtility.singleLineHeight;
            var detailPos = new Rect(position.x + 10, posY, position.width - 15, EditorGUIUtility.singleLineHeight);
            var detailProp = property.FindPropertyRelative("_detailName");

            var options = GetOptions(referenceModuleProp, GetTypes(property));
            if (options.Length == 0 || referenceModuleProp.objectReferenceValue == null)
            {
                EditorGUI.LabelField(detailPos, "⚠ No valid details available!");
            }
            else
            {
                var previousSelection = GetPreviousSelection(detailProp, options);
                var newSelection = EditorGUI.Popup(detailPos, "Detail", previousSelection, options);
                if (previousSelection != newSelection && newSelection >= 0 && newSelection < options.Length)
                {
                    detailProp.stringValue = options[newSelection];
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (referenceModuleProp.objectReferenceValue == null || options.Length == 0)
                {
                    detailProp.stringValue = null;
                }
            }
        }

        private int GetPreviousSelection(SerializedProperty detailProp, string[] options)
        {
            return Array.IndexOf(options, detailProp.stringValue);
        }

        private Type[] GetTypes(SerializedProperty property)
        {
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

        private string[] GetOptions(SerializedProperty referenceModuleProp, params Type[] types)
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
                    if (detail != null && types.Contains(detail.GetValueType()))
                    {
                        _options.Add(detail.GetName());
                    }
                }
            }

            return _options.ToArray();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 4;
        }
    }
}