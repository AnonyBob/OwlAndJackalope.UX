using System;
using System.Reflection;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References
{
    [CustomEditor(typeof(ReferenceModule))]
    public class ReferenceModuleEditor : UnityEditor.Editor
    {
        private SerializedReferenceTemplate _referenceTemplate;
        private const string SerializedReferenceProp = "_serializedReference";
        
        public override void OnInspectorGUI()
        {
            var previousGui = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = previousGui;
            
            EditorGUILayout.LabelField("Import Template");
            using (new EditorGUILayout.HorizontalScope())
            {
                _referenceTemplate = (SerializedReferenceTemplate)EditorGUILayout.ObjectField(_referenceTemplate,
                    typeof(SerializedReferenceTemplate), false);
                if (OJEditorUtility.Button("Import", Color.blue))
                {
                    ImportTemplate();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(SerializedReferenceProp), true);
            
            if (OJEditorUtility.CenteredButton("Save as new Template", Color.green, 250f))
            {
                SaveAsTemplate();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SaveAsTemplate()
        {
            var name = serializedObject.targetObject.name;
            var savePath = EditorUtility.SaveFilePanelInProject("Save as Template", $"{name}_template", "asset", null);
            if (!string.IsNullOrEmpty(savePath))
            {
                var newTemplate = CreateInstance<SerializedReferenceTemplate>();
                var serializedReference = GetSerializedReference();
                newTemplate.AddDetails(serializedReference.CopyDetails());
                
                AssetDatabase.CreateAsset(newTemplate, savePath);
                EditorGUIUtility.PingObject(newTemplate);
            }
        }

        private void ImportTemplate()
        {
            if (_referenceTemplate != null)
            {
                var serializedReference = GetSerializedReference();
                serializedReference.AddDetails(_referenceTemplate.CopyDetails());
            }
            
            _referenceTemplate = null;
        }

        private ISerializedReference GetSerializedReference()
        {
            var targetObject = serializedObject.targetObject;
            var field = typeof(ReferenceModule).GetField(SerializedReferenceProp, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                return (ISerializedReference)field.GetValue(targetObject);
            }

            throw new MissingFieldException(nameof(ReferenceModule), SerializedReferenceProp);
        }
    }
}