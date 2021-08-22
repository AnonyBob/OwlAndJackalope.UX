using System;
using System.Text.RegularExpressions;
using OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers;
using OwlAndJackalope.UX.Data.Serialized.Editor.EnumExtensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    public static class SharedDrawers
    {
        public const float Buffer = 5;
        public const string TypeString = "_type";
        public const string NameString = "_name";
        public const string CollectionString = "_collection";
        public const string ValueString = "_value";
        public const string StringValueString = "_stringValue";
        public const string ObjectValueString = "_referenceValue";
        public const string VectorValueString = "_vectorValue";
        public const string GameObjectValueString = "_gameObjectValue";
        public const string AssetReferenceValueString = "_assetReferenceValue";
        public const string SpriteValueString = "_spriteValue";
        public const string TextureValueString = "_textureValue";
        
        public const string EnumAssemblyString = "_enumAssemblyName";
        public const string EnumTypeString = "_enumTypeName";
        
        public const string ReferenceTemplatePath = "Reference";
        public const string ReferenceModulePath = "_reference";
        public const string ReferenceDetailsString = "_details";
        public const string ReferenceCollectionDetailsString = "_collectionDetails";
        public const string ReferenceMapDetailsString = "_mapDetails";
        
        public const string ConditionsString = "_conditions";

        public const string KeyTypeString = "_keyType";
        public const string ValueTypeString = "_valueType";

        public const string KeyCollectionString = "_keyCollection";
        public const string ValueCollectionString = "_valueCollection";
        
        public const string KeyEnumTypeString = "_keyEnumTypeName";
        public const string KeyEnumAssemblyString = "_keyEnumAssemblyName";
        public const string ValueEnumTypeString = "_valueEnumTypeName";
        public const string ValueEnumAssemblyString = "_valueEnumAssemblyName";
        
        public static SerializedProperty DrawTypeField(
            Rect position, 
            SerializedProperty property, 
            string typeString,
            string enumTypeString)
        {
            return DrawTypeField(position, property, typeString, enumTypeString, null);
        }

        public static SerializedProperty DrawTypeField(
            Rect position,
            SerializedProperty property,
            string typeString,
            string enumTypeString,
            string fieldName)
        {
            var typeProp = property.FindPropertyRelative(typeString);
            //var enumTypeProp = property.FindPropertyRelative(EnumTypeString);
            var detail = ((DetailType) typeProp.enumValueIndex);
            var displayString = detail != DetailType.Enum
                ? ((DetailType) typeProp.enumValueIndex).ToString()
                : GetEnumString(property, enumTypeString);
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
            if (enumProp != null && !string.IsNullOrEmpty(enumProp.stringValue))
            {
                var regex = new Regex(".*[\\.\\+](.*)");
                var groups = regex.Match(enumProp.stringValue).Groups;
                return groups[1].Value;
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

        public static (SerializedProperty enumTypeProp, SerializedProperty enumAssemblyProp) DrawEnumTypeField(
            Rect position, 
            SerializedProperty property,
            string enumTypeString,
            string enumAssemblyString)
        {
            return DrawEnumTypeField(position, property, enumTypeString, enumAssemblyString, null);
        }
        
        public static (SerializedProperty enumTypeProp, SerializedProperty enumAssemblyProp) DrawEnumTypeField(
            Rect position, 
            SerializedProperty property,
            string enumTypeString,
            string enumAssemblyString,
            string fieldName)
        {
            var previousEnabled = GUI.enabled;
            GUI.enabled = !InCollection(property) && !InCondition(property);
            
            var assemblyProp = property.FindPropertyRelative(enumAssemblyString);
            var enumTypeProp = property.FindPropertyRelative(enumTypeString);
            
            var index = Array.FindIndex(SerializedDetailEnumCache.EnumTypeFullNames, x => enumTypeProp.stringValue == x);
            index = Math.Max(0, index);
            
            index = EditorGUI.Popup(position, fieldName ?? string.Empty, index, SerializedDetailEnumCache.EnumTypeNames);   
            var type = SerializedDetailEnumCache.GetEnumType(SerializedDetailEnumCache.EnumTypeNames[index]);
            if (type != null)
            {
                assemblyProp.stringValue = type.Assembly.FullName;
                enumTypeProp.stringValue = type.FullName;
            }

            GUI.enabled = previousEnabled;
            return (enumTypeProp, assemblyProp);
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
                        foreach (var enumType in SerializedDetailEnumCache.EnumTypes)
                        {
                            menu.AddItem(new GUIContent($"{enumTypeString}/{enumType.Name}"), false, selection =>
                            {
                                InsertNewEnumItem(property, enumType, clearFunc);
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

        private static void InsertNewEnumItem(SerializedProperty property, Type enumType, Action<SerializedProperty> clearFunc)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            var newItem = property.GetArrayElementAtIndex(property.arraySize - 1);
            newItem.FindPropertyRelative(TypeString).enumValueIndex = (int)DetailType.Enum;
            newItem.FindPropertyRelative(EnumTypeString).stringValue = enumType.FullName;
            newItem.FindPropertyRelative(EnumAssemblyString).stringValue = enumType.Assembly.FullName;
            
            var guidString = Guid.NewGuid().ToString();
            newItem.FindPropertyRelative(NameString).stringValue =
                $"{enumType.Name} {guidString.Substring(0, guidString.IndexOf("-"))}"; 
            
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

                    newItem.FindPropertyRelative(KeyEnumTypeString).stringValue = keyType.EnumType?.FullName;
                    newItem.FindPropertyRelative(KeyEnumAssemblyString).stringValue = keyType.EnumType?.Assembly?.FullName;
                    
                    newItem.FindPropertyRelative(ValueEnumTypeString).stringValue = valueType.EnumType?.FullName;
                    newItem.FindPropertyRelative(ValueEnumAssemblyString).stringValue = valueType.EnumType?.Assembly?.FullName;
                    
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}