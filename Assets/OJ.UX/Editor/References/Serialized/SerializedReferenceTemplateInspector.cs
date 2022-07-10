using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomEditor(typeof(SerializedReferenceTemplate))]
    public class SerializedReferenceTemplateInspector : UnityEditor.Editor
    {
        private ReorderableList _list;
        private GenericMenu _menu;

        public override void OnInspectorGUI()
        {
            if (_list == null)
            {
                var detailsProp = serializedObject.FindProperty(nameof(SerializedReferenceTemplate.Details));
                _menu = SerializedReferenceUtility.CreateSelectionMenu(detailsProp);
                _list = SerializedReferenceUtility.CreateDetailList(new GUIContent("Details"), detailsProp, _menu);
            }
            
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}