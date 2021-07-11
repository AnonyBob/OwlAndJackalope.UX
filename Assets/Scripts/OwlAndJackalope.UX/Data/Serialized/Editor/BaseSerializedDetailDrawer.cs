﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    [CustomPropertyDrawer(typeof(BaseSerializedDetail))]
    public class BaseSerializedDetailDrawer : PropertyDrawer
    {
        protected class PropertyData
        {
            public DetailNameChecker NameChecker;
            public Assembly LoadedAssembly;
            public Type LoadedEnumType;
        }
        protected readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_data.TryGetValue(property.propertyPath, out var propertyData))
            {
                propertyData = new PropertyData()
                {
                    NameChecker = GetNameChecker(),
                };
                _data[property.propertyPath] = propertyData;
            }

            EditorGUI.BeginProperty(position, label, property);
            if (!SharedDrawers.InCollection(property))
            {
                var typePos = new Rect(position.x, position.y, position.width * 0.15f, EditorGUIUtility.singleLineHeight);
                var typeProp = SharedDrawers.DrawTypeField(typePos, property, SharedDrawers.TypeString, ClearPropValues);
                var namePos = new Rect(typePos.x + typePos.width + SharedDrawers.Buffer, position.y, 
                    position.width * 0.3f - SharedDrawers.Buffer, EditorGUIUtility.singleLineHeight);
                SharedDrawers.DrawNameField(namePos, property, SharedDrawers.NameString, propertyData.NameChecker);
                
                var remaining = new Rect(namePos.x + namePos.width + SharedDrawers.Buffer, position.y, 
                    position.width * 0.5f - SharedDrawers.Buffer, position.height);
                var type = (DetailType)typeProp.enumValueIndex;
                DrawForType(remaining, property, propertyData, type);
            }
            else
            {
                var remaining = new Rect(position.x, position.y, position.width, position.height);
                var type = (DetailType)property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                DrawForType(remaining, property, propertyData, type); 
            }

            EditorGUI.EndProperty();
        }

        private void DrawForType(Rect remaining, SerializedProperty property, PropertyData propertyData, DetailType type)
        {
            var valueProp = property.FindPropertyRelative(SharedDrawers.ValueString);
            var stringValueProp = property.FindPropertyRelative(SharedDrawers.StringValueString);
            var referenceProp = property.FindPropertyRelative(SharedDrawers.ReferenceValueString);
            var vectorValueProp = property.FindPropertyRelative(SharedDrawers.VectorValueString);
            var valuePosition = new Rect(remaining.x, remaining.y, remaining.width, EditorGUIUtility.singleLineHeight);
            
            switch (type)
            {
                case DetailType.Bool:
                    valueProp.doubleValue = EditorGUI.Toggle(valuePosition, GUIContent.none, valueProp.doubleValue > 0) ? 1 : 0;
                    break;
                case DetailType.Integer:
                    valueProp.doubleValue = EditorGUI.IntField(valuePosition, GUIContent.none, (int)Math.Floor(valueProp.doubleValue)) + 0.1;
                    break;
                case DetailType.Long:
                    valueProp.doubleValue = EditorGUI.LongField(valuePosition, GUIContent.none, (int)Math.Floor(valueProp.doubleValue)) + 0.1;
                    break;
                case DetailType.Float:
                    valueProp.doubleValue = EditorGUI.FloatField(valuePosition, GUIContent.none, (float)valueProp.doubleValue);
                    break;
                case DetailType.Double:
                    EditorGUI.PropertyField(valuePosition, valueProp, GUIContent.none);
                    break;
                case DetailType.Enum:
                    DrawEnumType(remaining, property, propertyData, valueProp);
                    break;
                case DetailType.String:
                    EditorGUI.PropertyField(valuePosition, stringValueProp,  GUIContent.none);
                    break;
                case DetailType.Reference:
                    EditorGUI.PropertyField(valuePosition, referenceProp, GUIContent.none);
                    if (referenceProp.objectReferenceValue == property.serializedObject.targetObject)
                    {
                        //Don't allow self reference. Its still possible for someone to double up if two different
                        //objects reference each other, but as a sanity check let's not let it happen here.
                        referenceProp.objectReferenceValue = null;
                    }
                    break;
                case DetailType.Vector2:
                    vectorValueProp.vector4Value = EditorGUI.Vector2Field(valuePosition, GUIContent.none, vectorValueProp.vector4Value);
                    break;
                case DetailType.Vector3:
                    vectorValueProp.vector4Value = EditorGUI.Vector3Field(valuePosition, GUIContent.none, vectorValueProp.vector4Value);
                    break;
                case DetailType.Color:
                    vectorValueProp.vector4Value = EditorGUI.ColorField(valuePosition, GUIContent.none, vectorValueProp.vector4Value);
                    break;
                case DetailType.Custom:
                    DrawCustomType(remaining, property);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void DrawEnumType(Rect position, SerializedProperty property, PropertyData propertyData, SerializedProperty valueProp)
        {
            var selectButtonPos = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            var (enumTypeProp, assemblyProp) =
                SharedDrawers.DrawEnumTypeField(selectButtonPos, property, SharedDrawers.EnumTypeString, SharedDrawers.EnumAssemblyString);
            
            var valuePos = new Rect(selectButtonPos.x + selectButtonPos.width + SharedDrawers.Buffer, position.y, 
                position.width * 0.5f - SharedDrawers.Buffer, position.height);
            var assemblyName = assemblyProp.stringValue;
            var enumTypeName = enumTypeProp.stringValue;
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(enumTypeName))
            {
                if (propertyData.LoadedEnumType == null || propertyData.LoadedEnumType.FullName != enumTypeName)
                {
                    propertyData.LoadedAssembly = Assembly.Load(assemblyName);
                    propertyData.LoadedEnumType = propertyData.LoadedAssembly.GetType(enumTypeName);
                }

                if (propertyData.LoadedEnumType != null)
                {
                    var value = (int)Math.Floor(valueProp.doubleValue);
                    var updatedValue = Convert.ToInt32(EditorGUI.EnumPopup(valuePos, (Enum)Enum.ToObject(propertyData.LoadedEnumType, value)));
                    valueProp.doubleValue = updatedValue + 0.1;
                }
                else
                {
                    EditorGUI.LabelField(valuePos, $"Could not load {enumTypeName}");
                }
            }
        }

        private void ClearPropValues(SerializedProperty property)
        {
            property.FindPropertyRelative(SharedDrawers.ValueString).doubleValue = 0;
            property.FindPropertyRelative(SharedDrawers.StringValueString).stringValue = string.Empty;
            property.FindPropertyRelative(SharedDrawers.ReferenceValueString).objectReferenceValue = null;
            property.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value = Vector4.zero;
            property.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue = string.Empty;
            property.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue = string.Empty;

            _data.Remove(property.propertyPath);
            ClearCustomValues(property);
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ExperiencePath);
        }
        
        protected virtual void DrawCustomType(Rect position, SerializedProperty property)
        {
            //OVERRIDE TO HANDLE ADDITIONAL TYPES!
            EditorGUI.LabelField(position, "Not Supported");
        }

        protected virtual void ClearCustomValues(SerializedProperty property)
        {
            //OVERRIDE TO HANDLE CLEARING CUSTOM VALUES
        }
    }
}