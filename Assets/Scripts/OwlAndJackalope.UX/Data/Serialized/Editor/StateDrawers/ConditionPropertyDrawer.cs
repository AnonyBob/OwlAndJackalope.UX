using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Conditions;
using OwlAndJackalope.UX.Conditions.Serialized;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.StateDrawers
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
            var parameterWidth = pos.width * 0.3f - SharedDrawers.Buffer;
            var equalityWidth = pos.width * 0.2f - SharedDrawers.Buffer;
            var p1Pos = new Rect(pos.x, pos.y, parameterWidth, pos.height);
            var detailName = DrawParameter(p1Pos, property, ParamOneName, ParameterType.Detail);
            var detailType = GetDetailType(property, detailName);
            
            property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex = detailType.DetailType;
            property.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue = detailType.EnumType;
            property.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue = detailType.EnumAssembly;

            var eqPos = new Rect(p1Pos.x + p1Pos.width + SharedDrawers.Buffer, pos.y, equalityWidth, pos.height);
            DrawComparison(eqPos, property);
            
            var p2Pos = new Rect(eqPos.x + eqPos.width + SharedDrawers.Buffer, pos.y, pos.width - parameterWidth - equalityWidth, pos.height);
            DrawParameter(p2Pos, property, ParamTwoName, null, detailName);
        }

        private string DrawParameter(Rect position, SerializedProperty property, string parameterName,
            ParameterType? forceType = null, string referenceDetail = null)
        {
            var parameter = property.FindPropertyRelative(parameterName);
            var valueProp = property.FindPropertyRelative(SharedDrawers.ValueString);
            var nameProp = parameter.FindPropertyRelative("Name");
            var typeProp = parameter.FindPropertyRelative("Type");
            
            var typePos = new Rect(position.x, position.y, position.width * 0.25f - SharedDrawers.Buffer, position.height);
            var namePos = new Rect(typePos.x + SharedDrawers.Buffer + typePos.width, position.y, 
                position.width * 0.75f - SharedDrawers.Buffer, position.height);
            var type = forceType ?? (ParameterType) typeProp.enumValueIndex;
            
            if (forceType.HasValue)
            {
                typeProp.enumValueIndex = (int)forceType.Value;
                namePos = typePos;
                namePos.width = position.width;
            }
            else
            {
                typeProp.enumValueIndex = EditorGUI.Popup(typePos, typeProp.enumValueIndex, GetTypeOptions(typeProp).ToArray());
            }
            
            if (type == ParameterType.Detail)
            {
                var detailType = referenceDetail != null ? GetDetailType(property, referenceDetail).DetailType : -1;
                var options = GetDetailOptions(property, detailType).ToArray();
                if (options.Length == 0)
                {
                    return string.Empty;
                }
                var detailIndex = Math.Max(0, Array.IndexOf(options, nameProp.stringValue));
                var nextIndex = EditorGUI.Popup(namePos, detailIndex, options);
                if (string.IsNullOrEmpty(nameProp.stringValue) || nextIndex != detailIndex || (nextIndex < options.Length && options[nextIndex] != nameProp.stringValue))
                {
                    nameProp.stringValue = options[nextIndex];
                }
            }
            else if (type == ParameterType.Value)
            {
                if (!string.IsNullOrEmpty(referenceDetail))
                {
                    var detailType = GetDetailType(property, referenceDetail);
                    valueProp.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex = detailType.DetailType;
                    valueProp.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue = detailType.EnumType;
                    valueProp.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue =
                        detailType.EnumAssembly;
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

        private (int DetailType, string EnumType, string EnumAssembly) GetDetailType(SerializedProperty property, string detailName)
        {
            var details = GetDetailsProperty(property);
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detail = details.GetArrayElementAtIndex(i);
                if (detail.FindPropertyRelative(SharedDrawers.NameString).stringValue == detailName)
                {
                    var detailType = detail.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                    var enumType = detail.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue;
                    var assembly = detail.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue;

                    return (detailType, enumType, assembly);
                }
            }

            return ((int)DetailType.Bool, null, null);
        }
        
        private IEnumerable<string> GetDetailOptions(SerializedProperty property, int detailType)
        {
            var details = GetDetailsProperty(property);
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detail = details.GetArrayElementAtIndex(i);
                var type = detail.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                if (detailType == type || (detailType < 0 && ((DetailType)type).IsComparable()))
                {
                    yield return detail.FindPropertyRelative(SharedDrawers.NameString).stringValue;
                }
            }
        }

        private IEnumerable<string> GetTypeOptions(SerializedProperty property)
        {
            yield return "V";
            yield return "D";
            if (!property.propertyPath.Contains("_states"))
            {
                yield return "A"; //Arguments are only accessible in actions.
            }
        }

        private SerializedProperty GetDetailsProperty(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject as MonoBehaviour;
            var serializedReference = new SerializedObject(target.GetComponentInParent<ReferenceModule>());
            return serializedReference.FindProperty($"{SharedDrawers.ReferenceModulePath}.{SharedDrawers.ReferenceDetailsString}");
        }
    }
}