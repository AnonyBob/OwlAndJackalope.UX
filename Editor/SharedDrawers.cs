using System;
using System.Text.RegularExpressions;
using OwlAndJackalope.UX.Editor.DetailDrawers;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor
{
    public static class SharedDrawers
    {
        public const float Buffer = 5;
        
        //Name and type paths
        public const string TypeString = "_type";
        public const string NameString = "_name";
        public const string KeyTypeString = "_keyType";
        public const string ValueTypeString = "_valueType";
        
        //Value paths
        public const string ValueString = "_value";
        public const string StringValueString = "_stringValue";
        public const string ObjectValueString = "_referenceValue";
        public const string VectorValueString = "_vectorValue";
        public const string AssetReferenceValueString = "_assetReferenceValue";

        //Reference paths
        public const string ReferenceTemplatePath = "Reference";
        public const string ReferenceModulePath = "_reference";
        public const string ReferenceDetailsString = "_details";
        public const string ReferenceCollectionDetailsString = "_collectionDetails";
        public const string ReferenceMapDetailsString = "_mapDetails";

        //Collection paths
        
        public const string CollectionString = "_collection";
        public const string KeyCollectionString = "_keyCollection";
        public const string ValueCollectionString = "_valueCollection";
        
        //Enum id paths
        public const string EnumIdString = "_enumId";
        public const string KeyEnumIdString = "_keyEnumId";
        public const string ValueEnumIdString = "_valueEnumId";
        
        public const string ConditionsString = "_conditions";

        public static bool Button(string text, Color color, params GUILayoutOption[] options)
        {
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            var value = GUILayout.Button(text, options);
            GUI.backgroundColor = originalColor;
            return value;
        }
        
        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return GUILayout.Button(text, options);
        }
        
        public static SerializedProperty DrawTypeField(
            Rect position, 
            SerializedProperty property, 
            string typeString,
            string enumIdString)
        {
            return DrawTypeField(position, property, typeString, enumIdString, null);
        }

        public static SerializedProperty DrawTypeField(
            Rect position,
            SerializedProperty property,
            string typeString,
            string enumIdString,
            string fieldName)
        {
            var typeProp = property.FindPropertyRelative(typeString);
            //var enumTypeProp = property.FindPropertyRelative(EnumTypeString);
            var detail = ((DetailType) typeProp.enumValueIndex);
            var displayString = detail != DetailType.Enum
                ? ((DetailType) typeProp.enumValueIndex).ToString()
                : GetEnumString(property, enumIdString);
            if (fieldName != null)
            {
                EditorGUI.LabelField(position, fieldName, displayString, EditorStyles.helpBox);    
            }
            else
            {
                EditorGUI.LabelField(position, displayString, EditorStyles.helpBox);    
            }

            return typeProp;
        }
        
        private static string GetEnumString(SerializedProperty property, string enumTypeString)
        {
            var enumProp = property.FindPropertyRelative(enumTypeString);
            if (enumProp != null)
            {
                var creator = SerializedDetailEnumCache.GetCreator(enumProp.intValue);
                return creator?.EnumName ?? string.Empty;
            }
            return "";
        }

        public static SerializedProperty DrawNameField
            (Rect position, 
            SerializedProperty property, 
            string nameString, 
            DetailNameChecker checker)
        {
            var enabled = GUI.enabled;
            GUI.enabled = !Application.isPlaying;
            var nameProp = property.FindPropertyRelative(nameString);
            var previousName = nameProp.stringValue;
            var newName = EditorGUI.TextField(position, GUIContent.none, previousName);
            if (previousName != newName)
            {
                nameProp.stringValue = checker.CheckName(previousName, newName, property);    
            }
            
            GUI.enabled = enabled;

            return nameProp;
        }

        public static bool InCollection(SerializedProperty property)
        {
            var collectionRegex = new Regex("_collectionDetails.Array.*_collection.Array");
            var mapRegex = new Regex("_mapDetails.Array.*Collection.Array");
            return collectionRegex.IsMatch(property.propertyPath) || mapRegex.IsMatch(property.propertyPath);
        }

        public static bool InCondition(SerializedProperty property)
        {
            return property.propertyPath.Contains("_conditions");
        }

        public static float GetCollectionHeight(SerializedProperty property, string collectionString)
        {
            var rowSize = EditorGUIUtility.singleLineHeight + Buffer;
            var collections = property.FindPropertyRelative(collectionString);
            return rowSize * (collections.isExpanded ? (4 + Mathf.Max(1,collections.arraySize)) : 2);
        }
        
        public static float GetMapHeight(SerializedProperty property, string collectionString)
        {
            var rowSize = EditorGUIUtility.singleLineHeight + Buffer;
            var collections = property.FindPropertyRelative(collectionString);
            return rowSize * (collections.isExpanded ? (4 + Mathf.Max(2,collections.arraySize * 2)) : 2);
        }

        public static ReorderableList CreateDetailList(SerializedProperty property,
            bool allowReordering,
            Action<SerializedProperty> clearFunc)
        {
            var list = new ReorderableList(property.serializedObject, 
                property, 
                allowReordering, 
                false, 
                true, 
                true);

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index));
            };

            list.onAddDropdownCallback = (rect, reorderableList) =>
            {
                var type = typeof(DetailType);
                var menu = new GenericMenu();
                var enumTypeString = DetailType.Enum.ToString();
                foreach (var val in type.GetEnumNames())
                {
                    if (val != enumTypeString)
                    {
                        menu.AddItem(new GUIContent(val), false, selection =>
                        {
                            var enumName = (string) selection;
                            var detailType = (DetailType)Enum.Parse(type, enumName);
                            InsertNewItem(property, detailType, clearFunc);
                        }, val);
                    }
                    else
                    {
                        foreach (var enumType in SerializedDetailEnumCache.EnumTypeNames)
                        {
                            menu.AddItem(new GUIContent($"{enumTypeString}/{enumType}"), false, selection =>
                            {
                                var enumDetails = SerializedDetailEnumCache.GetCreator(enumType);
                                InsertNewEnumItem(property, enumDetails.EnumId, enumDetails.Creator.EnumName, clearFunc);
                            }, enumType);
                        }
                    }
                    
                }
                menu.DropDown(rect);
            };
            
            return list;
        }

        private static void InsertNewItem(SerializedProperty property, DetailType detailType, Action<SerializedProperty> clearFunc)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            var newItem = property.GetArrayElementAtIndex(property.arraySize - 1);
            newItem.FindPropertyRelative(TypeString).enumValueIndex = (int)detailType;
            
            var guidString = Guid.NewGuid().ToString();
            newItem.FindPropertyRelative(NameString).stringValue =
                $"{detailType} {guidString.Substring(0, guidString.IndexOf("-"))}"; 
            
            clearFunc?.Invoke(newItem);
            property.serializedObject.ApplyModifiedProperties();
        }

        private static void InsertNewEnumItem(SerializedProperty property, int enumId, string enumName, Action<SerializedProperty> clearFunc)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            var newItem = property.GetArrayElementAtIndex(property.arraySize - 1);
            newItem.FindPropertyRelative(TypeString).enumValueIndex = (int)DetailType.Enum;
            newItem.FindPropertyRelative(EnumIdString).intValue = enumId;

            var guidString = Guid.NewGuid().ToString();
            newItem.FindPropertyRelative(NameString).stringValue =
                $"{enumName} {guidString.Substring(0, guidString.IndexOf("-"))}"; 
            
            clearFunc?.Invoke(newItem);
            property.serializedObject.ApplyModifiedProperties();
        }

        public static ReorderableList CreateMapList(SerializedProperty property)
        {
            var list = new ReorderableList(property.serializedObject, property, false, false, true, true);
            list.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true);
            };
            list.elementHeightCallback = index =>
            {
                var prop = property.GetArrayElementAtIndex(index);
                return GetMapHeight(prop, KeyCollectionString);
            };
            list.onAddDropdownCallback = (rect, list) =>
            {
                SelectMapTypeWindow.Open(rect, (keyType, valueType) =>
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    var newItem = property.GetArrayElementAtIndex(property.arraySize - 1);
                    
                    newItem.FindPropertyRelative(KeyTypeString).enumValueIndex = (int)keyType.MainType;
                    newItem.FindPropertyRelative(ValueTypeString).enumValueIndex = (int)valueType.MainType;

                    newItem.FindPropertyRelative(KeyEnumIdString).intValue = keyType.EnumDetails.EnumId;
                    newItem.FindPropertyRelative(ValueEnumIdString).intValue = valueType.EnumDetails.EnumId;

                    newItem.FindPropertyRelative(KeyCollectionString).ClearArray();
                    newItem.FindPropertyRelative(ValueCollectionString).ClearArray();
                    
                    var guidString = Guid.NewGuid().ToString();
                    newItem.FindPropertyRelative(NameString).stringValue =
                        $"{keyType.ToString()} {guidString.Substring(0, guidString.IndexOf("-"))}";
                    property.serializedObject.ApplyModifiedProperties();
                });
            };

            return list;
        }
        
        public static void ToggleAddAndRemove(ReorderableList list, bool isPlaying)
        {
            list.displayAdd = !isPlaying;
            list.displayRemove = !isPlaying;
            list.draggable = !isPlaying;
        }
        
        public static void UpdateValueInBaseDetail(SerializedProperty property, DetailType type, object value)
        {
            var valueProp = property.FindPropertyRelative(ValueString);
            var vectorValueProp = property.FindPropertyRelative(VectorValueString);
            switch (type)
            {
                case DetailType.Bool:
                    valueProp.doubleValue = ((bool) value) ? 1 : -1;
                    break;
                case DetailType.Integer:
                    valueProp.doubleValue = ((int) value) + 0.1;
                    break;
                case DetailType.Long:
                    valueProp.doubleValue = ((long) value) + 0.1;
                    break;
                case DetailType.Float:
                    valueProp.doubleValue = (float) value;
                    break;
                case DetailType.Double:
                    valueProp.doubleValue = (double) value;
                    break;
                case DetailType.Enum:
                    valueProp.doubleValue = ((int) value) + 0.1;
                    break;
                case DetailType.String:
                    property.FindPropertyRelative(StringValueString).stringValue = ((string) value);
                    break;
                case DetailType.Reference:
                    break;
                case DetailType.Vector2:
                    vectorValueProp.vector4Value = (Vector2) value;
                    break;
                case DetailType.Vector3:
                    vectorValueProp.vector4Value = (Vector3) value;
                    break;
                case DetailType.Color:
                    vectorValueProp.vector4Value = (Color) value;
                    break;
                case DetailType.GameObject:
                    property.FindPropertyRelative(ObjectValueString).objectReferenceValue =
                        (GameObject) value;
                    break;
                case DetailType.Texture:
                    property.FindPropertyRelative(ObjectValueString).objectReferenceValue =
                        (Texture2D) value;
                    break;
                case DetailType.Sprite:
                    property.FindPropertyRelative(ObjectValueString).objectReferenceValue =
                        (Sprite) value;
                    break;
                case DetailType.AssetReference:
                    break;
                case DetailType.TimeSpan:
                    var timespan = (TimeSpan) value;
                    valueProp.doubleValue = timespan.Ticks + 0.1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}