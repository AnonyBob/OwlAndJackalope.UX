using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(BaseSerializedMapDetail))]
    public class BaseMapDetailDrawer : PropertyDrawer
    {
        protected class PropertyData
        {
            public DetailNameChecker NameChecker;
            public ReorderableList List;
        }
        protected readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();
        
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            if (!_data.TryGetValue(property.propertyPath, out var propertyData))
            {
                propertyData = new PropertyData()
                {
                    NameChecker = GetNameChecker(),
                    List = CreateList(property)
                };
                _data[property.propertyPath] = propertyData;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            var namePos = new Rect(position.x, position.y + SharedDrawers.Buffer, position.width, EditorGUIUtility.singleLineHeight);
            SharedDrawers.DrawNameField(namePos, property, SharedDrawers.NameString, propertyData.NameChecker);
            
            var keyTypeProp = property.FindPropertyRelative(SharedDrawers.KeyTypeString);
            var keyIsEnum = (DetailType)keyTypeProp.enumValueIndex == DetailType.Enum;
            var keyTypeNamePos = new Rect(position.x, 
                namePos.y + namePos.height + SharedDrawers.Buffer, 
                position.width * 0.25f, 
                EditorGUIUtility.singleLineHeight);
            var keyTypePos = new Rect(keyTypeNamePos.x + keyTypeNamePos.width + SharedDrawers.Buffer, 
                keyTypeNamePos.y, 
                keyTypeNamePos.width - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);
            var keyEnumPos = new Rect(keyTypePos.x + keyTypePos.width + SharedDrawers.Buffer, 
                keyTypePos.y, 
                position.width * 0.5f - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);

            var valueTypeProp = property.FindPropertyRelative(SharedDrawers.ValueTypeString);
            var valueIsEnum = (DetailType) valueTypeProp.enumValueIndex == DetailType.Enum;
            var valueTypeNamePos = new Rect(position.x, 
                keyTypeNamePos.y + keyTypeNamePos.height + SharedDrawers.Buffer, 
                position.width * 0.25f, 
                EditorGUIUtility.singleLineHeight);
            var valueTypePos = new Rect(valueTypeNamePos.x + valueTypeNamePos.width + SharedDrawers.Buffer, 
                valueTypeNamePos.y, 
                position.width * 0.25f - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);
            var valueEnumPos = new Rect(valueTypePos.x + valueTypePos.width + SharedDrawers.Buffer, 
                valueTypePos.y, 
                position.width * 0.5f - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(keyTypeNamePos, "Key Type");
            EditorGUI.LabelField(valueTypeNamePos, "Value Type");
            SharedDrawers.DrawTypeField(keyTypePos, property, SharedDrawers.KeyTypeString, SharedDrawers.KeyEnumTypeString);
            SharedDrawers.DrawTypeField(valueTypePos, property, SharedDrawers.ValueTypeString, SharedDrawers.ValueEnumTypeString);

            var collectionY = valueTypePos.y + valueTypePos.height + SharedDrawers.Buffer;
            var collectionHeaderPos = new Rect(position.x + SharedDrawers.Buffer * 3, collectionY, 
                position.width - SharedDrawers.Buffer * 3, position.height - collectionY);
            
            if (EditorGUI.PropertyField(collectionHeaderPos, property.FindPropertyRelative(SharedDrawers.KeyCollectionString), false))
            {
                var collectionListPos = new Rect(collectionHeaderPos.x, 
                    collectionHeaderPos.y + EditorGUIUtility.singleLineHeight + SharedDrawers.Buffer, 
                    collectionHeaderPos.width, collectionHeaderPos.height);
                propertyData.List.DoList(collectionListPos);
            }
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SharedDrawers.GetMapHeight(property, SharedDrawers.KeyCollectionString);
        }

        private ReorderableList CreateList(SerializedProperty property)
        {
            var keyCollectionProp = property.FindPropertyRelative(SharedDrawers.KeyCollectionString);
            var valueCollectionProp = property.FindPropertyRelative(SharedDrawers.ValueCollectionString);
            var list = new ReorderableList(property.serializedObject, 
                keyCollectionProp, 
                true, 
                false, 
                true, 
                true);

            list.onAddCallback = reorderableList =>
            {
                keyCollectionProp.InsertArrayElementAtIndex(keyCollectionProp.arraySize);
                valueCollectionProp.InsertArrayElementAtIndex(valueCollectionProp.arraySize);
                
                var keyProp = keyCollectionProp.GetArrayElementAtIndex(keyCollectionProp.arraySize - 1);
                var valueProp = valueCollectionProp.GetArrayElementAtIndex(valueCollectionProp.arraySize - 1);
                keyProp.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                    property.FindPropertyRelative(SharedDrawers.KeyTypeString).enumValueIndex;
                keyProp.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.KeyEnumTypeString).stringValue;
                keyProp.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.KeyEnumTypeString).stringValue;
                
                valueProp.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                    property.FindPropertyRelative(SharedDrawers.ValueTypeString).enumValueIndex;
                valueProp.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.ValueEnumTypeString).stringValue;
                valueProp.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.ValueEnumAssemblyString).stringValue;
            };

            list.onRemoveCallback = reorderableList =>
            {
                keyCollectionProp.DeleteArrayElementAtIndex(reorderableList.index);
                valueCollectionProp.DeleteArrayElementAtIndex(reorderableList.index);
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                var keyLabelRect = new Rect(rect.x, rect.y, rect.width * 0.25f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(keyLabelRect, "Key");

                var keyRect = new Rect(keyLabelRect.x + keyLabelRect.width + SharedDrawers.Buffer, 
                    rect.y, rect.width * 0.75f, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(keyRect, keyCollectionProp.GetArrayElementAtIndex(index));

                var valueLabelRect = new Rect(rect.x, keyLabelRect.y + keyLabelRect.height, 
                    rect.width * 0.25f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(valueLabelRect, "Value");
                
                var valueRect = new Rect(valueLabelRect.x + valueLabelRect.width + SharedDrawers.Buffer, 
                    valueLabelRect.y + SharedDrawers.Buffer * 0.33f, rect.width * 0.75f, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(valueRect, valueCollectionProp.GetArrayElementAtIndex(index));
            };

            list.onReorderCallbackWithDetails = (reorderableList, index, newIndex) =>
            {
                //keyCollectionProp.MoveArrayElement(index, newIndex);
                valueCollectionProp.MoveArrayElement(index, newIndex);
            };

            list.elementHeight = EditorGUIUtility.singleLineHeight * 2 + SharedDrawers.Buffer;
            
            return list;
        }

        protected virtual void ClearPropValues(SerializedProperty property)
        {
            property.FindPropertyRelative(SharedDrawers.KeyCollectionString).arraySize = 0;
            property.FindPropertyRelative(SharedDrawers.ValueCollectionString).arraySize = 0;
        }

        protected virtual void ClearKeyPropValues(SerializedProperty property)
        {
            property.FindPropertyRelative(SharedDrawers.KeyEnumTypeString).stringValue = string.Empty;
            property.FindPropertyRelative(SharedDrawers.KeyEnumAssemblyString).stringValue = string.Empty;
            ClearPropValues(property);
        }

        protected virtual void ClearValuePropValues(SerializedProperty property)
        {
            property.FindPropertyRelative(SharedDrawers.ValueEnumTypeString).stringValue = string.Empty;
            property.FindPropertyRelative(SharedDrawers.ValueEnumAssemblyString).stringValue = string.Empty;
            ClearPropValues(property);
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ReferenceModulePath);
        }
    }
}