using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Conditions;
using OwlAndJackalope.UX.Conditions.Serialized;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    [CustomPropertyDrawer(typeof(BaseSerializedCondition))]
    public class ConditionPropertyDrawer : PropertyDrawer
    {
        private const string ParamOneName = "_parameterOne";
        private const string ParamTwoName = "_parameterTwo";
        private const string ComparisonTypeName = "_comparisonType";
        
        public override void OnGUI(Rect pos, SerializedProperty property,
            GUIContent label)
        {
            var options =  GetDetailOptions(property).ToArray();

            var parameterWidth = pos.width * 0.4f - SharedDrawers.Buffer;
            var equalityWidth = pos.width * 0.2f - SharedDrawers.Buffer;
            var p1Pos = new Rect(pos.x, pos.y, parameterWidth, pos.height);
            var detailName = DrawParameter(p1Pos, property, ParamOneName, options, ParameterType.Detail);
            
            var eqPos = new Rect(p1Pos.x + p1Pos.width + SharedDrawers.Buffer, pos.y, equalityWidth, pos.height);
            DrawComparison(eqPos, property);
            
            var p2Pos = new Rect(eqPos.x + eqPos.width + SharedDrawers.Buffer, pos.y, parameterWidth, pos.height);
            DrawParameter(p2Pos, property, ParamTwoName, options, null, detailName);
        }

        private string DrawParameter(Rect position, SerializedProperty property, string parameterName, string[] options, 
            ParameterType? forceType = null, string referenceDetail = null)
        {
            var parameter = property.FindPropertyRelative(parameterName);
            var valueProp = property.FindPropertyRelative(SharedDrawers.ValueString);
            var nameProp = parameter.FindPropertyRelative("Name");
            var typeProp = parameter.FindPropertyRelative("Type");
            
            var typePos = new Rect(position.x, position.y, position.width * 0.25f - SharedDrawers.Buffer, position.height);
            var namePos = new Rect(typePos.x + SharedDrawers.Buffer + typePos.width, position.y, position.width * 0.75f - SharedDrawers.Buffer, position.height);
            var type = forceType ?? (ParameterType) typeProp.enumValueIndex;
            
            if (forceType.HasValue)
            {
                typeProp.enumValueIndex = (int)forceType.Value;
                namePos = typePos;
                namePos.width = position.width;
            }
            else
            {
                EditorGUI.PropertyField(typePos, typeProp, GUIContent.none);
            }
            
            if (type == ParameterType.Detail)
            {
                var detailIndex = Math.Max(0, Array.IndexOf(options, nameProp.stringValue));
                var nextIndex = EditorGUI.Popup(namePos, detailIndex, options);
                if (nextIndex != detailIndex && nextIndex >= 0 && nextIndex < options.Length)
                {
                    nameProp.stringValue = options[nextIndex];
                }
            }
            else if (type == ParameterType.Value)
            {
                if (!string.IsNullOrEmpty(referenceDetail))
                {
                    valueProp.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                        GetDetailType(property, referenceDetail);
                }
                EditorGUI.PropertyField(namePos, valueProp);
            }

            return nameProp.stringValue;
        }

        private void DrawComparison(Rect position, SerializedProperty property)
        {
            var comparison = property.FindPropertyRelative(ComparisonTypeName);
            var comparisonType = comparison.enumValueIndex;
            comparison.enumValueIndex = EditorGUI.Popup(position, comparisonType, ComparisonExtensions.AsString);
        }

        private int GetDetailType(SerializedProperty property, string detailName)
        {
            var details = property.serializedObject.FindProperty($"{SharedDrawers.ExperiencePath}.{SharedDrawers.ReferenceDetailsString}");
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detail = details.GetArrayElementAtIndex(i);
                if (detail.FindPropertyRelative(SharedDrawers.NameString).stringValue == detailName)
                {
                    return detail.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                }
            }

            return (int)DetailType.Bool;
        }
        
        private IEnumerable<string> GetDetailOptions(SerializedProperty property)
        {
            var details = property.serializedObject.FindProperty($"{SharedDrawers.ExperiencePath}.{SharedDrawers.ReferenceDetailsString}");
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detail = details.GetArrayElementAtIndex(i);
                var type = (DetailType)detail.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                if (type.IsComparable())
                {
                    yield return detail.FindPropertyRelative(SharedDrawers.NameString).stringValue;
                }
            }
        }
    }
}