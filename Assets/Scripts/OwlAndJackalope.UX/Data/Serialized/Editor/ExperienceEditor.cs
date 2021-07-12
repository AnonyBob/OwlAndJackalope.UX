using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    [CustomEditor(typeof(Experience))]
    public class ExperienceEditor : UnityEditor.Editor
    {
        private ReferenceEditor _referenceEditor;
        private ReferenceTemplate _referenceTemplate;
        
        public override void OnInspectorGUI()
        {
            if (_referenceEditor == null)
            {
                _referenceEditor = new ReferenceEditor(serializedObject, "_referenceModule._reference.");
            }
            
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                EditorGUILayout.LabelField("Reference Module", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                using (new EditorGUILayout.HorizontalScope())
                {
                    _referenceTemplate = (ReferenceTemplate)EditorGUILayout.ObjectField(_referenceTemplate, 
                        typeof(ReferenceTemplate), false);
                    if (GUILayout.Button("Add Details"))
                    {
                        _referenceTemplate = null;
                    }
                }
                
                _referenceEditor.Draw();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_stateModule"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}