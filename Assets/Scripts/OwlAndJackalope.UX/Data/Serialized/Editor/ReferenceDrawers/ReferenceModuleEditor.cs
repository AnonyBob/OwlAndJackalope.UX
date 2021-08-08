using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.ReferenceDrawers
{
    [CustomEditor(typeof(ReferenceModule))]
    public class ReferenceModuleEditor : UnityEditor.Editor
    {
        private ReferenceEditor _referenceEditor;
        public override void OnInspectorGUI()
        {
            if (_referenceEditor == null)
            {
                _referenceEditor = new ReferenceEditor(serializedObject, "_reference.");
            }
            
            _referenceEditor.Draw();
            serializedObject.ApplyModifiedProperties();
        }
    }
}