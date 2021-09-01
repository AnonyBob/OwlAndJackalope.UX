using System.Collections;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(BaseSerializedMapDetail))]
    public class BaseMapDetailDrawer : PropertyDrawer
    {
        protected class PropertyData : BasePropertyData
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
            propertyData.HandleRuntimeDetailChanged(property, UpdateValues);
            
            EditorGUI.BeginProperty(position, label, property);

            var keyTypePos = new Rect(position.x, 
                position.y + SharedDrawers.Buffer, 
                position.width * 0.25f, 
                EditorGUIUtility.singleLineHeight);
            
            var valueTypePos = new Rect(keyTypePos.x + keyTypePos.width + SharedDrawers.Buffer, 
                keyTypePos.y, 
                position.width * 0.25f - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);
            
            SharedDrawers.DrawTypeField(keyTypePos, property, SharedDrawers.KeyTypeString, SharedDrawers.KeyEnumTypeString);
            SharedDrawers.DrawTypeField(valueTypePos, property, SharedDrawers.ValueTypeString, SharedDrawers.ValueEnumTypeString);
            
            var namePos = new Rect(valueTypePos.x + valueTypePos.width + SharedDrawers.Buffer,
                position.y + SharedDrawers.Buffer, position.width * 0.5f - SharedDrawers.Buffer, 
                EditorGUIUtility.singleLineHeight);
            SharedDrawers.DrawNameField(namePos, property, SharedDrawers.NameString, propertyData.NameChecker);

            var collectionY = valueTypePos.y + valueTypePos.height + SharedDrawers.Buffer;
            var collectionHeaderPos = new Rect(position.x + SharedDrawers.Buffer * 3, collectionY, 
                position.width - SharedDrawers.Buffer * 3, position.height - collectionY);
            
            if (EditorGUI.PropertyField(collectionHeaderPos, property.FindPropertyRelative(SharedDrawers.KeyCollectionString), false))
            {
                var collectionListPos = new Rect(collectionHeaderPos.x, 
                    collectionHeaderPos.y + EditorGUIUtility.singleLineHeight + SharedDrawers.Buffer, 
                    collectionHeaderPos.width, collectionHeaderPos.height);
                
                SharedDrawers.ToggleAddAndRemove(propertyData.List, Application.isPlaying);
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
                AddItem(property, keyCollectionProp, valueCollectionProp);
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

        private void UpdateValues(SerializedProperty property, BasePropertyData propertydata)
        {
            var value = propertydata.RuntimeDetail.GetObject() as IDictionary;
            var keyType = (DetailType)property.FindPropertyRelative(SharedDrawers.KeyTypeString).enumValueIndex;
            var valueType = (DetailType)property.FindPropertyRelative(SharedDrawers.ValueTypeString).enumValueIndex;
            var keyCollectionProp = property.FindPropertyRelative(SharedDrawers.KeyCollectionString);
            var valueCollectionProp = property.FindPropertyRelative(SharedDrawers.ValueCollectionString);
            keyCollectionProp.ClearArray();
            valueCollectionProp.ClearArray();

            if (value != null)
            {
                foreach (var key in value.Keys)
                {
                    var newItem = AddItem(property, keyCollectionProp, valueCollectionProp);
                    SharedDrawers.UpdateValueInBaseDetail(newItem.key, keyType, key);
                    SharedDrawers.UpdateValueInBaseDetail(newItem.value, valueType, value[key]);
                }
            }
        }

        private (SerializedProperty key, SerializedProperty value) AddItem(SerializedProperty property, 
            SerializedProperty keyCollectionProp, SerializedProperty valueCollectionProp)
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
                property.FindPropertyRelative(SharedDrawers.KeyEnumAssemblyString).stringValue;
                
            valueProp.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                property.FindPropertyRelative(SharedDrawers.ValueTypeString).enumValueIndex;
            valueProp.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue =
                property.FindPropertyRelative(SharedDrawers.ValueEnumTypeString).stringValue;
            valueProp.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue =
                property.FindPropertyRelative(SharedDrawers.ValueEnumAssemblyString).stringValue;

            return (keyProp, valueProp);
        }
        
        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ReferenceModulePath);
        }
    }
}