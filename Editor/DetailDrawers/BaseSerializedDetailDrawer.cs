using System;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEditor;
using UnityEngine;
#if USE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace OwlAndJackalope.UX.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(BaseSerializedDetail))]
    public class BaseSerializedDetailDrawer : PropertyDrawer
    {
        protected class PropertyData : BasePropertyData
        {
            public DetailNameChecker NameChecker;
        }
        protected readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();

        public static void Clear(SerializedProperty newItem)
        {
            newItem.FindPropertyRelative(SharedDrawers.ValueString).doubleValue = 0;
            newItem.FindPropertyRelative(SharedDrawers.StringValueString).stringValue = null;
            newItem.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue = null;
            newItem.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value = Vector4.zero;
            newItem.FindPropertyRelative(SharedDrawers.AssetReferenceValueString).FindPropertyRelative("m_AssetGUID")
                .stringValue = null;
        }
        
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
            propertyData.HandleRuntimeDetailChanged(property, UpdateValue);
            
            EditorGUI.BeginProperty(position, label, property);
            if (!SharedDrawers.InCollection(property) && !SharedDrawers.InCondition(property))
            {
                var typePos = new Rect(position.x, position.y, position.width * 0.15f, EditorGUIUtility.singleLineHeight);
                var typeProp = SharedDrawers.DrawTypeField(typePos, property, SharedDrawers.TypeString, SharedDrawers.EnumIdString);
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
            var referenceProp = property.FindPropertyRelative(SharedDrawers.ObjectValueString);
            var vectorValueProp = property.FindPropertyRelative(SharedDrawers.VectorValueString);
            var assetRefValueProp = property.FindPropertyRelative(SharedDrawers.AssetReferenceValueString);
            var valuePosition = new Rect(remaining.x, remaining.y, remaining.width, EditorGUIUtility.singleLineHeight);

            var enabled = GUI.enabled;
            GUI.enabled = !Application.isPlaying || propertyData.RuntimeDetail is IMutableDetail;
            
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
                    if (Application.isPlaying)
                    {
                        var value = referenceProp.objectReferenceValue != null ? referenceProp.objectReferenceValue.name : "None";
                        var reference = propertyData.RuntimeDetail?.GetObject() as IReference;
                        value = reference?.ShortPrint() ?? value;
                        EditorGUI.LabelField(valuePosition, value);
                    }
                    else
                    {
                        referenceProp.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none,
                            referenceProp.objectReferenceValue, typeof(ReferenceTemplate), true);
                        if (referenceProp.objectReferenceValue == property.serializedObject.targetObject)
                        {
                            //Don't allow self reference. Its still possible for someone to double up if two different
                            //objects reference each other, but as a sanity check let's not let it happen here.
                            referenceProp.objectReferenceValue = null;
                        } 
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
                    referenceProp.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none,
                        referenceProp.objectReferenceValue, typeof(GameObject), true);
                    break;
                case DetailType.AssetReference:
#if USE_ADDRESSABLES
                    if (Application.isPlaying)
                    {
                        var value = assetRefValueProp.FindPropertyRelative("m_AssetGUID").stringValue;
                        var runtimeReference = propertyData.RuntimeDetail?.GetObject() as AssetReference;
                        value = runtimeReference?.AssetGUID ?? value;
                        EditorGUI.LabelField(valuePosition, value);
                    }
                    else
                    {
                        var previousWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 0.01f;
                        EditorGUI.PropertyField(valuePosition, assetRefValueProp, GUIContent.none);
                        EditorGUIUtility.labelWidth = previousWidth;
                    }
#else
                    EditorGUI.LabelField(valuePosition, "Add Addressables for Support");
#endif
                    break;
                case DetailType.Sprite:
                    referenceProp.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none,
                        referenceProp.objectReferenceValue, typeof(Sprite), true);
                    break;
                case DetailType.Texture:
                    referenceProp.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none,
                        referenceProp.objectReferenceValue, typeof(Texture2D), true);
                    break;
                case DetailType.TimeSpan:
                    var timeSpan = TimeSpan.FromTicks((long) Math.Floor(valueProp.doubleValue));
                    EditorGUIUtility.labelWidth = 10;
                    var width = valuePosition.width / 4;
                    var dayPos = new Rect(valuePosition.x, valuePosition.y, width, valuePosition.height);
                    var hourPos = new Rect(valuePosition.x + width, valuePosition.y, width, valuePosition.height);
                    var minPos = new Rect(valuePosition.x + width * 2, valuePosition.y, width, valuePosition.height);
                    var secPos = new Rect(valuePosition.x + width  * 3, valuePosition.y, width, valuePosition.height);
                    var days = EditorGUI.IntField(dayPos, "d", timeSpan.Days);
                    var hours = EditorGUI.IntField(hourPos,"h", timeSpan.Hours);
                    var mins = EditorGUI.IntField(minPos, "m", timeSpan.Minutes);
                    var secs = EditorGUI.IntField(secPos, "s", timeSpan.Seconds);
                    EditorGUIUtility.labelWidth = 0;
                        
                    timeSpan = new TimeSpan(days, hours, mins, secs);
                    valueProp.doubleValue = timeSpan.Ticks + 0.1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                if (propertyData.RuntimeDetail is IMutableDetail mutable)
                {
                    mutable.SetObject(GetValue(property, propertyData, type));
                }
            }

            GUI.enabled = enabled;
        }

        private void DrawEnumType(Rect position, SerializedProperty property, PropertyData propertyData, SerializedProperty valueProp)
        {
            var enumIdProp = property.FindPropertyRelative(SharedDrawers.EnumIdString);
            var enumType = SerializedDetailEnumCache.GetCreator(enumIdProp.intValue)?.EnumType;
            if (enumType != null)
            {
                var value = (int)Math.Floor(valueProp.doubleValue);
                var updatedValue = Convert.ToInt32(EditorGUI.EnumPopup(position, (Enum)Enum.ToObject(enumType, value)));
                valueProp.doubleValue = updatedValue + 0.1;
            }
            else
            {
                EditorGUI.LabelField(position, $"Could not load enum with id: {enumIdProp}");
            }
        }

        private void UpdateValue(SerializedProperty property, BasePropertyData propertyData)
        {
            var value = propertyData.RuntimeDetail.GetObject();
            var type = (DetailType)property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            SharedDrawers.UpdateValueInBaseDetail(property, type, value);
        }

        private object GetValue(SerializedProperty property, PropertyData propertyData, DetailType type)
        {
            var valueProp = property.FindPropertyRelative(SharedDrawers.ValueString);
            var vectorValueProp = property.FindPropertyRelative(SharedDrawers.VectorValueString);
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
                    return (property.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue 
                        as ReferenceTemplate)?.Reference?.ConvertToReference();
                case DetailType.Vector2:
                    return (Vector2)vectorValueProp.vector4Value;
                case DetailType.Vector3:
                    return (Vector3)vectorValueProp.vector4Value;
                case DetailType.Color:
                    return (Color)vectorValueProp.vector4Value;
                case DetailType.GameObject:
                    return property.FindPropertyRelative(SharedDrawers.ObjectValueString)
                        .objectReferenceValue as GameObject;
                case DetailType.Texture:
                    return property.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue as
                        Texture2D;
                case DetailType.Sprite:
                    return property.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue as
                        Sprite;
#if USE_ADDRESSABLES
                case DetailType.AssetReference:
                    return new AssetReference(property.FindPropertyRelative(SharedDrawers.AssetReferenceValueString)
                        .FindPropertyRelative("m_AssetGUID").stringValue);
#endif
                case DetailType.TimeSpan:
                    return TimeSpan.FromTicks((long) Math.Floor(valueProp.doubleValue));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private object GetEnumValue(SerializedProperty property, PropertyData propertyData, int enumValue)
        {
            var creator = SerializedDetailEnumCache.GetCreator(property.FindPropertyRelative(SharedDrawers.EnumIdString).intValue);
            return creator?.Convert(enumValue) ?? DetailType.Bool;
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ReferenceModulePath);
        }
    }
}