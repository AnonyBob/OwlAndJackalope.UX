using System;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.ReferenceDrawers
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
                if (GUILayout.Button("Import"))
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
            
            AddDetails(details, _referenceEditor.DetailListProperty, CopyDetail);
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

        private void CopyDetail(SerializedProperty src, SerializedProperty dst)
        {
            dst.FindPropertyRelative(SharedDrawers.NameString).stringValue = src.FindPropertyRelative(SharedDrawers.NameString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex = src.FindPropertyRelative(SharedDrawers.TypeString).enumValueIndex;
            dst.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue = src.FindPropertyRelative(SharedDrawers.EnumTypeString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue = src.FindPropertyRelative(SharedDrawers.EnumAssemblyString).stringValue;
            
            dst.FindPropertyRelative(SharedDrawers.ValueString).doubleValue = src.FindPropertyRelative(SharedDrawers.ValueString).doubleValue;
            dst.FindPropertyRelative(SharedDrawers.StringValueString).stringValue = src.FindPropertyRelative(SharedDrawers.StringValueString).stringValue;
            dst.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue = src.FindPropertyRelative(SharedDrawers.ObjectValueString).objectReferenceValue;
            dst.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value = src.FindPropertyRelative(SharedDrawers.VectorValueString).vector4Value;
            dst.FindPropertyRelative(SharedDrawers.AssetReferenceValueString).vector4Value = src.FindPropertyRelative(SharedDrawers.AssetReferenceValueString).vector4Value;

        }
    }
}