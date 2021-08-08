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
        public const string ReferenceValueString = "_referenceValue";
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

        public const string ExperienceStatesPath = "_stateModule._states";
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
            return rowSize * (collections.isExpanded ? (6 + Mathf.Max(2,collections.arraySize * 2)) : 4);
        }

        public static ReorderableList CreateDetailList(SerializedProperty property, 
            string typeString, 
            string nameString,
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
                foreach (var val in type.GetEnumNames())
                {
                    menu.AddItem(new GUIContent(val), false, selection =>
                    {
                        var enumName = (string) selection;
                        property.InsertArrayElementAtIndex(property.arraySize);
                        var newItem = property.GetArrayElementAtIndex(property.arraySize - 1);
                        newItem.FindPropertyRelative(typeString).enumValueIndex = (int)Enum.Parse(type, enumName);

                        var guidString = Guid.NewGuid().ToString();
                        newItem.FindPropertyRelative(nameString).stringValue =
                            $"{enumName} {guidString.Substring(0, guidString.IndexOf("-"))}";
                        
                        clearFunc?.Invoke(newItem);

                        property.serializedObject.ApplyModifiedProperties();
                    }, val);
                }
                menu.DropDown(rect);
            };
            
            return list;
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
                    
                    newItem.FindPropertyRelative(KeyTypeString).enumValueIndex = (int)keyType;
                    newItem.FindPropertyRelative(ValueTypeString).enumValueIndex = (int)valueType;
                    
                    var guidString = Guid.NewGuid().ToString();
                    newItem.FindPropertyRelative(NameString).stringValue =
                        $"{keyType.ToString()} {guidString.Substring(0, guidString.IndexOf("-"))}";
                    property.serializedObject.ApplyModifiedProperties();
                });
            };

            return list;
        }
    }
}