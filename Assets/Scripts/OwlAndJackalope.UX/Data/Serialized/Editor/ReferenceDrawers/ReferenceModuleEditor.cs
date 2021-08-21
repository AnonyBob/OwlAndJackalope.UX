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
            var 
            
            var serializedReference = new SerializedObject(_referenceTemplate);
            var details = serializedReference.FindProperty(SharedDrawers.ReferenceDetailsString);
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detail = details.GetArrayElementAtIndex(i);
                seri
            }
        }
    }
}