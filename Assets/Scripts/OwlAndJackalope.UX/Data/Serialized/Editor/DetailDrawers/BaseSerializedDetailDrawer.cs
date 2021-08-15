﻿using System;
using System.Collections.Generic;
using System.Reflection;
using OwlAndJackalope.UX.Data.Extensions;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers
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
            if (!SharedDrawers.InCollection(property) && !SharedDrawers.InCondition(property))
            {
                var typePos = new Rect(position.x, position.y, position.width * 0.15f, EditorGUIUtility.singleLineHeight);
                var typeProp = SharedDrawers.DrawTypeField(typePos, property, SharedDrawers.TypeString, SharedDrawers.EnumTypeString);
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
            var gameObjectValueProp = property.FindPropertyRelative(SharedDrawers.GameObjectValueString);
            var assetRefValueProp = property.FindPropertyRelative(SharedDrawers.AssetReferenceValueString);
            var spriteValueProp = property.FindPropertyRelative(SharedDrawers.SpriteValueString);
            var textureValueProp = property.FindPropertyRelative(SharedDrawers.TextureValueString);
            var valuePosition = new Rect(remaining.x, remaining.y, remaining.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.BeginChangeCheck();
            switch (type)
            {
                case DetailType.Bool:
                    valueProp.doubleValue = EditorGUI.Toggle(valuePosition, GUIContent.none, valueProp.doubleValue > 0) ? 1 : 0;
                    break;
                case DetailType.Integer:
                    valueProp.doubleValue = EditorGUI.IntField(valuePosition, GUIContent.none, (int)Math.Floor(valueProp.doubleValue)) + 0.1;
                    break;
                case DetailType.Long:
                    valueProp.doubleValue = EditorGUI.LongField(valuePosition, GUIContent.none, (long)Math.Floor(valueProp.doubleValue)) + 0.1;
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
                case DetailType.GameObject:
                    gameObjectValueProp.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none,
                        gameObjectValueProp.objectReferenceValue, typeof(GameObject), true);
                    break;
                case DetailType.AssetReference:
                    var previousWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 0.01f;
                    EditorGUI.PropertyField(valuePosition, assetRefValueProp, GUIContent.none);
                    EditorGUIUtility.labelWidth = previousWidth;
                    break;
                case DetailType.Sprite:
                    EditorGUI.PropertyField(valuePosition, spriteValueProp, GUIContent.none);
                    break;
                case DetailType.Texture:
                    EditorGUI.PropertyField(valuePosition, textureValueProp, GUIContent.none);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                var module = property.serializedObject.targetObject as ReferenceModule;
                if (module != null)
                {
                    var name = property.FindPropertyRelative(SharedDrawers.NameString).stringValue;
                    var detail = module.Reference.GetDetail(name) as IMutableDetail;
                    detail?.SetObject(GetValue(property, propertyData, type));
                    
                    Debug.Log(module.Reference);
                }
            }
        }

        private void DrawEnumType(Rect position, SerializedProperty property, PropertyData propertyData, SerializedProperty valueProp)
        {
            var enumTypeProp = property.FindPropertyRelative(SharedDrawers.EnumTypeString);
            var assemblyProp = property.FindPropertyRelative(SharedDrawers.EnumAssemblyString);

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
                    var updatedValue = Convert.ToInt32(EditorGUI.EnumPopup(position, (Enum)Enum.ToObject(propertyData.LoadedEnumType, value)));
                    valueProp.doubleValue = updatedValue + 0.1;
                }
                else
                {
                    EditorGUI.LabelField(position, $"Could not load {enumTypeName}");
                }
            }
        }

        private object GetValue(SerializedProperty property, PropertyData propertyData, DetailType type)
        {
            var valueProp = property.FindPropertyRelative(SharedDrawers.ValueString);
            var vectorValueProp = property.FindPropertyRelative(SharedDrawers.VectorValueString);
            var gameObjectValueProp = property.FindPropertyRelative(SharedDrawers.GameObjectValueString);
            var assetRefValueProp = property.FindPropertyRelative(SharedDrawers.AssetReferenceValueString);
            var spriteValueProp = property.FindPropertyRelative(SharedDrawers.SpriteValueString);
            var textureValueProp = property.FindPropertyRelative(SharedDrawers.TextureValueString);
            switch (type)
            {
                case DetailType.Bool:
                    return valueProp.doubleValue > 0;
                case DetailType.Integer:
                    return (int) Math.Floor(valueProp.doubleValue);
                case DetailType.Long:
                    return (long) Math.Floor(valueProp.doubleValue);
                case DetailType.Float:
                    return (float) valueProp.doubleValue;
                case DetailType.Double:
                    return valueProp.doubleValue;
                case DetailType.Enum:
                    return GetEnumValue(property,  propertyData, (int)Math.Floor(valueProp.doubleValue));
                case DetailType.String:
                    return property.FindPropertyRelative(SharedDrawers.StringValueString).stringValue;
                case DetailType.Reference:
                    return (property.FindPropertyRelative(SharedDrawers.ReferenceValueString).objectReferenceValue 
                        as ReferenceTemplate)?.Reference?.ConvertToReference();
                case DetailType.Vector2:
                    return (Vector2)vectorValueProp.vector4Value;
                case DetailType.Vector3:
                    return (Vector3)vectorValueProp.vector4Value;
                case DetailType.Color:
                    return (Color)vectorValueProp.vector4Value;
                case DetailType.GameObject:
                    return property.FindPropertyRelative(SharedDrawers.GameObjectValueString)
                        .objectReferenceValue as GameObject;
                case DetailType.Texture:
                    return property.FindPropertyRelative(SharedDrawers.TextureValueString).objectReferenceValue as
                        Texture2D;
                case DetailType.Sprite:
                    return property.FindPropertyRelative(SharedDrawers.SpriteValueString).objectReferenceValue as
                        Sprite;
                case DetailType.AssetReference:
                    return new AssetReference(property.FindPropertyRelative(SharedDrawers.AssetReferenceValueString)
                        .FindPropertyRelative("m_AssetGUID").stringValue);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private object GetEnumValue(SerializedProperty property, PropertyData propertyData, int enumValue)
        {
            if (propertyData.LoadedEnumType != null)
            {
                return Enum.ToObject(propertyData.LoadedEnumType, enumValue);
            }

            return DetailType.Bool;
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ReferenceModulePath);
        }
    }
}