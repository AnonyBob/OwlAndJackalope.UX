using System;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.ReferenceDrawers
{
    [CustomEditor(typeof(ReferenceModule))]
    public class ReferenceModuleEditor : UnityEditor.Editor
    {
        private ReferenceEditor _referenceEditor;
        private ReferenceTemplate _referenceTemplate;
        
        public override void OnInspectorGUI()
        {
            if (_referenceEditor == null)
            {
                _referenceEditor = new ReferenceEditor(serializedObject, "_reference.");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _referenceTemplate = (ReferenceTemplate)EditorGUILayout.ObjectField("Import Reference", _referenceTemplate,
                    typeof(ReferenceTemplate), false);

                GUI.enabled = _referenceTemplate != null;
                if (SharedDrawers.Button("Import", Color.blue, GUILayout.Width(100)))
                {
                    ImportReferenceTemplate();
                }

                GUI.enabled = true;
            }
            
            
            _referenceEditor.Draw();
            serializedObject.ApplyModifiedProperties();
        }

        private void ImportReferenceTemplate()
        {
            var serializedReference = new SerializedObject(_referenceTemplate);
            var details = serializedReference.FindProperty("Reference." + SharedDrawers.ReferenceDetailsString);
            var collectionDetails = serializedReference.FindProperty("Reference." + SharedDrawers.ReferenceCollectionDetailsString);
            var mapDetails = serializedReference.FindProperty("Reference." + SharedDrawers.ReferenceMapDetailsString);
            
            AddDetails(details, _referenceEditor.DetailListProperty, CopyDetail);
            AddDetails(collectionDetails, _referenceEditor.CollectionListProperty, CopyCollectionDetail);
            AddDetails(mapDetails, _referenceEditor.MapListProperty, CopyMapDetail);
        }

        private void AddDetails(SerializedProperty incomingDetails, SerializedProperty localDetails,
            Action<SerializedProperty, SerializedProperty> copyTo)
        {
            for (var i = 0; i < incomingDetails.arraySize; ++i)
            {
                var detail = incomingDetails.GetArrayElementAtIndex(i);
                var localDetail = GetLocalDetail(localDetails, detail.FindPropertyRelative(SharedDrawers.NameString).stringValue);
                if (localDetail == null)
                {
                    localDetails.InsertArrayElementAtIndex(localDetails.arraySize);
                    localDetail = localDetails.GetArrayElementAtIndex(localDetails.arraySize - 1);
                }

                copyTo(detail, localDetail);
            }
        }

        private SerializedProperty GetLocalDetail(SerializedProperty localDetails, string name)
        {
            for (var i = 0; i < localDetails.arraySize; ++i)
            {
                var localDetail = localDetails.GetArrayElementAtIndex(i);
                var localName = localDetail.FindPropertyRelative(SharedDrawers.NameString).stringValue;
                if (name == localName)
                {
                    return localDetail;
                }
            }

            return null;
        }

        private void CopyCollectionDetail(SerializedProperty src, SerializedProperty dst)
        {
            dst.FindPropertyRelative(SharedDrawers.NameString).stringValue = src.FindPropertyRelative(SharedDrawers.NameString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex = src.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            dst.FindPropertyRelative(SharedDrawers.EnumIdString).intValue = src.FindPropertyRelative(SharedDrawers.EnumIdString).intValue;

            var srcCollections = src.FindPropertyRelative(SharedDrawers.CollectionString);
            var dstCollections = dst.FindPropertyRelative(SharedDrawers.CollectionString);
            dstCollections.ClearArray();
            for (var i = 0; i < srcCollections.arraySize; ++i)
            {
                var srcItem = srcCollections.GetArrayElementAtIndex(i);
                dstCollections.InsertArrayElementAtIndex(i);
                var dstItem = dstCollections.GetArrayElementAtIndex(i);
                CopyDetail(srcItem, dstItem);
            }
        }
        
        private void CopyMapDetail(SerializedProperty src, SerializedProperty dst)
        {
            dst.FindPropertyRelative(SharedDrawers.NameString).stringValue = src.FindPropertyRelative(SharedDrawers.NameString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.KeyTypeString).enumValueIndex = src.FindPropertyRelative(SharedDrawers.KeyTypeString).enumValueIndex;
            dst.FindPropertyRelative(SharedDrawers.ValueTypeString).enumValueIndex = src.FindPropertyRelative(SharedDrawers.ValueTypeString).enumValueIndex;
            
            dst.FindPropertyRelative(SharedDrawers.KeyEnumIdString).intValue = src.FindPropertyRelative(SharedDrawers.KeyEnumIdString).intValue;
            dst.FindPropertyRelative(SharedDrawers.ValueEnumIdString).intValue = src.FindPropertyRelative(SharedDrawers.ValueEnumIdString).intValue;

            var srcKeys = src.FindPropertyRelative(SharedDrawers.KeyCollectionString);
            var srcValues = src.FindPropertyRelative(SharedDrawers.ValueCollectionString);
            
            var dstKeys = dst.FindPropertyRelative(SharedDrawers.KeyCollectionString);
            var dstValues = dst.FindPropertyRelative(SharedDrawers.ValueCollectionString);
            
            dstKeys.ClearArray();
            dstValues.ClearArray();
            for (var i = 0; i < srcKeys.arraySize; ++i)
            {
                var srcItemKey = srcKeys.GetArrayElementAtIndex(i);
                var srcItemValue = srcValues.GetArrayElementAtIndex(i);
                
                dstKeys.InsertArrayElementAtIndex(i);
                dstValues.InsertArrayElementAtIndex(i);
                
                var dstItemKey = dstKeys.GetArrayElementAtIndex(i);
                var dstItemValue = dstValues.GetArrayElementAtIndex(i);
                
                CopyDetail(srcItemKey, dstItemKey);
                CopyDetail(srcItemValue, dstItemValue);
            }
        }
        
        private void CopyDetail(SerializedProperty src, SerializedProperty dst)
        {
            dst.FindPropertyRelative(SharedDrawers.NameString).stringValue = src.FindPropertyRelative(SharedDrawers.NameString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex = src.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            dst.FindPropertyRelative(SharedDrawers.EnumIdString).intValue = src.FindPropertyRelative(SharedDrawers.EnumIdString).intValue;

            dst.FindPropertyRelative(SharedDrawers.ValueString).doubleValue = src.FindPropertyRelative(SharedDrawers.ValueString).doubleValue;
            dst.FindPropertyRelative(SharedDrawers.StringValueString).stringValue = src.FindPropertyRelative(SharedDrawers.StringValueString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue = src.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue;
            dst.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value = src.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value;
            dst.FindPropertyRelative(SharedDrawers.AssetReferenceValueString).FindPropertyRelative("m_AssetGUID").stringValue = 
                src.FindPropertyRelative(SharedDrawers.AssetReferenceValueString).FindPropertyRelative("m_AssetGUID").stringValue;
        }
    }
}