using System;
using OJ.UX.Runtime;
using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomPropertyDrawer(typeof(ConditionEnumComparison), true)]
    public class ConditionEnumValueComparisonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pos = new Rect(position);
            pos.width = pos.width * 0.5f - 2f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("_comparisonType"), GUIContent.none);

            pos.x = pos.x + pos.width + 2f;

            var comparisonValue = property.FindPropertyRelative("_comparisonValue");
            var currentValue = comparisonValue.intValue;

            var enumType = GetEnumType(property);
            if (enumType != null)
            {
                var selectedEnumValue = (Enum)Enum.Parse(enumType, currentValue.ToString());
                selectedEnumValue = EditorGUI.EnumPopup(pos, GUIContent.none, selectedEnumValue);
                comparisonValue.intValue = (int)(object)selectedEnumValue;
            }
        }

        private Type GetEnumType(SerializedProperty property)
        {
            var conditionProp = property.FindParentProperty();
            if (conditionProp != null)
            {
                var observerProp = conditionProp.FindPropertyRelative("_observer");
                var referenceProp = observerProp.FindPropertyRelative("_referenceModule");
                var detailNameProp = observerProp.FindPropertyRelative("_detailName");

                if (referenceProp != null && referenceProp.objectReferenceValue is ReferenceModule module)
                {
                    var referenceObject = new SerializedObject(module);
                    var detailsProp = referenceObject.FindProperty("_serializedReference").FindPropertyRelative("Details");
                    for (var i = 0; i < detailsProp.arraySize; ++i)
                    {
                        var detailProp = detailsProp.GetArrayElementAtIndex(i);
                        var nameProp = detailProp.FindPropertyRelative("Name");
                        if (nameProp != null && nameProp.stringValue == detailNameProp.stringValue)
                        {
                            return ((ISerializedDetail)detailProp.managedReferenceValue).GetValueType();
                        }
                    }
                }
            }

            return null;
        }
    }
}