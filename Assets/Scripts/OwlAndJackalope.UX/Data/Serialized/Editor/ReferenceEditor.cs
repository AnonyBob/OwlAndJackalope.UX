using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    public class ReferenceEditor
    {
        private const string DetailPath = "_details";
        private const string CollectionsPath = "_collectionDetails";
        private const string MapsPath = "_mapDetails";
        
        private readonly ReorderableList _detailList;
        private readonly ReorderableList _collectionDetailList;
        private readonly ReorderableList _mapDetailList;

        private readonly SerializedProperty _detailListProp;
        private readonly SerializedProperty _collectionListProp;
        private readonly SerializedProperty _mapListProp;
        private readonly SerializedObject _serializedObject;
        
        private static readonly object[] Empty = new object[0];
        private static readonly MethodInfo CacheMethod = typeof(ReorderableList).GetMethod("ClearCacheRecursive",
            BindingFlags.Instance | BindingFlags.NonPublic);

        public ReferenceEditor(SerializedObject serializedObject, string propertyNamePrefix = "")
        {
            _serializedObject = serializedObject;
            _detailListProp = _serializedObject.FindProperty(propertyNamePrefix + DetailPath);
            _collectionListProp = _serializedObject.FindProperty(propertyNamePrefix + CollectionsPath);
            _mapListProp = _serializedObject.FindProperty(propertyNamePrefix + MapsPath);

            _detailList = SharedDrawers.CreateDetailList(_detailListProp, SharedDrawers.TypeString, SharedDrawers.NameString, true,null);
            _collectionDetailList = SharedDrawers.CreateDetailList(_collectionListProp, SharedDrawers.TypeString, SharedDrawers.NameString,
                false, newItem =>
                {
                    newItem.FindPropertyRelative(SharedDrawers.CollectionString).ClearArray();
                });
            _collectionDetailList.elementHeightCallback = index =>
            {
                var prop = _collectionListProp.GetArrayElementAtIndex(index);
                return SharedDrawers.GetCollectionHeight(prop, SharedDrawers.CollectionString);
            };

            _mapDetailList = SharedDrawers.CreateMapList(_mapListProp);
        }
        
        public void Draw()
        {
            if (EditorGUILayout.PropertyField(_detailListProp, false))
            {
                _detailList.DoLayoutList();    
            }
            
            if (EditorGUILayout.PropertyField(_collectionListProp, false))
            {
                _collectionDetailList.DoLayoutList();
                CacheMethod.Invoke(_collectionDetailList, Empty);
            }

             if (EditorGUILayout.PropertyField(_mapListProp, false))
            {
                _mapDetailList.DoLayoutList();
                CacheMethod.Invoke(_mapDetailList, Empty);
            }
        }
    }
}