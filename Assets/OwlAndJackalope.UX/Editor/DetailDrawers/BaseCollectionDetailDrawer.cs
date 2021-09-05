using System.Collections;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(BaseSerializedCollectionDetail))]
    public class BaseCollectionDetailDrawer : PropertyDrawer
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
            
            var typePos = new Rect(position.x, position.y + SharedDrawers.Buffer, position.width * 0.15f, EditorGUIUtility.singleLineHeight);
            SharedDrawers.DrawTypeField(typePos, property, SharedDrawers.TypeString, SharedDrawers.EnumIdString);
            
            var namePos = new Rect(typePos.x + typePos.width + SharedDrawers.Buffer, 
                position.y + SharedDrawers.Buffer, position.width * 0.8f, EditorGUIUtility.singleLineHeight);
            SharedDrawers.DrawNameField(namePos, property, SharedDrawers.NameString, propertyData.NameChecker);
            
            var collectionY = typePos.y + typePos.height + SharedDrawers.Buffer;
            var collectionHeaderPos = new Rect(position.x + SharedDrawers.Buffer * 3, collectionY, 
                position.width - SharedDrawers.Buffer * 3, position.height - collectionY);
            
            if (EditorGUI.PropertyField(collectionHeaderPos, property.FindPropertyRelative(SharedDrawers.CollectionString), false))
            {
                var collectionListPos = new Rect(collectionHeaderPos.x, 
                    collectionHeaderPos.y + EditorGUIUtility.singleLineHeight + SharedDrawers.Buffer, 
                    collectionHeaderPos.width, collectionHeaderPos.height);
                
                SharedDrawers.ToggleAddAndRemove(propertyData.List, Application.isPlaying);
                propertyData.List.DoList(collectionListPos);
            }

            EditorGUI.EndProperty();
        }

        private void UpdateValues(SerializedProperty property, BasePropertyData propertydata)
        {
            var value = propertydata.RuntimeDetail.GetObject() as IList;
            var type = (DetailType)property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            var collections = property.FindPropertyRelative(SharedDrawers.CollectionString);
            collections.ClearArray();

            if (value != null)
            {
                foreach (var item in value)
                {
                    var newItem = AddItem(property, collections);
                    SharedDrawers.UpdateValueInBaseDetail(newItem, type, item);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return SharedDrawers.GetCollectionHeight(property, SharedDrawers.CollectionString);
        }

        private ReorderableList CreateList(SerializedProperty property)
        {
            var collectionProp = property.FindPropertyRelative(SharedDrawers.CollectionString);
            var list = new ReorderableList(property.serializedObject, 
                collectionProp, 
                true, 
                false, 
                true, 
                true);

            list.onAddCallback = reorderableList =>
            {
                AddItem(property, collectionProp);
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, collectionProp.GetArrayElementAtIndex(index));
            };
            
            return list;
        }

        private SerializedProperty AddItem(SerializedProperty property, SerializedProperty collectionProp)
        {
            collectionProp.InsertArrayElementAtIndex(collectionProp.arraySize);
            var newItem = collectionProp.GetArrayElementAtIndex(collectionProp.arraySize - 1);
            newItem.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            newItem.FindPropertyRelative(SharedDrawers.EnumIdString).intValue =
                property.FindPropertyRelative(SharedDrawers.EnumIdString).intValue;
            return newItem;
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ReferenceModulePath);
        }
    }
}