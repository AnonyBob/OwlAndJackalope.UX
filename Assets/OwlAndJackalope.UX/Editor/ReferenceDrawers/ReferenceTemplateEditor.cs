using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEditor;

namespace OwlAndJackalope.UX.Editor.ReferenceDrawers
{
    [CustomEditor(typeof(ReferenceTemplate))]
    public class ReferenceTemplateEditor : UnityEditor.Editor
    {
        private ReferenceEditor _referenceEditor;
        public override void OnInspectorGUI()
        {
            if (_referenceEditor == null)
            {
                _referenceEditor = new ReferenceEditor(serializedObject, "Reference.");
            }
            
            _referenceEditor.Draw();
            serializedObject.ApplyModifiedProperties();
        }
    }
}