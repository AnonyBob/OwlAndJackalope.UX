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
        private StateModuleEditor _statesEditor;
        
        private bool _referenceOpen = true;
        private bool _statesOpen = true;
        private GUIStyle _boldFoldoutStyle;

        public override void OnInspectorGUI()
        {
            if (_referenceEditor == null)
            {
                _referenceEditor = new ReferenceEditor(serializedObject, "_referenceModule._reference.");
                _boldFoldoutStyle = new GUIStyle(EditorStyles.foldoutHeader);
                _boldFoldoutStyle.fontStyle = FontStyle.Bold;
                _boldFoldoutStyle.fontSize = 16;
            }

            if (_statesEditor == null)
            {
                _statesEditor = new StateModuleEditor(serializedObject);
            }
            
            _referenceOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_referenceOpen, " Reference Module", _boldFoldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (_referenceOpen)
            {
                using (new EditorGUILayout.VerticalScope("helpbox"))
                {
                    EditorGUILayout.Space();
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
                }
            }

            EditorGUILayout.Space();
            
            _statesOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_statesOpen, " States Module", _boldFoldoutStyle);
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (_statesOpen)
            {
                using (new EditorGUILayout.VerticalScope("helpbox"))
                {
                    EditorGUILayout.Space();
                    _statesEditor.Draw();
                } 
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}