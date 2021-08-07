using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    [CustomPropertyDrawer(typeof(BaseSerializedCollectionDetail))]
    public class BaseCollectionDetailDrawer : PropertyDrawer
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
            
            var typePos = new Rect(position.x, position.y + SharedDrawers.Buffer, position.width * 0.1f, EditorGUIUtility.singleLineHeight);
            SharedDrawers.DrawTypeField(typePos, property, SharedDrawers.TypeString, SharedDrawers.EnumTypeString);
            
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
                propertyData.List.DoList(collectionListPos);
            }

            EditorGUI.EndProperty();
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
                collectionProp.InsertArrayElementAtIndex(collectionProp.arraySize);
                var prop = collectionProp.GetArrayElementAtIndex(collectionProp.arraySize - 1);
                prop.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex =
                    property.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
                prop.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue;
                prop.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue =
                    property.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue;
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, collectionProp.GetArrayElementAtIndex(index));
            };
            
            return list;
        }

        protected virtual void ClearPropValues(SerializedProperty property)
        {
            property.FindPropertyRelative(SharedDrawers.CollectionString).arraySize = 0;
            property.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue = string.Empty;
            property.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue = string.Empty;
        }

        protected virtual DetailNameChecker GetNameChecker()
        {
            return new DetailNameChecker(SharedDrawers.ReferenceTemplatePath, SharedDrawers.ExperiencePath);
        }
    }
}