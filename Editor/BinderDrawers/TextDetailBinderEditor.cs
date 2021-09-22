using OwlAndJackalope.UX.Runtime.DetailBinders;
using UnityEditor;

namespace OwlAndJackalope.UX.Editor.BinderDrawers
{
    [CustomEditor(typeof(TextDetailBinder))]
    public class TextDetailBinderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var useDefaultString = serializedObject.FindProperty("_useDefaultString");
            EditorGUILayout.PropertyField(useDefaultString);

            if (useDefaultString.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultString"));    
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseStringObserver"));  
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_stringArgumentObservers"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}