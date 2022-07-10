using OJ.UX.Runtime.Binders;
using UnityEditor;

namespace OJ.UX.Editor.Binders
{
    [CustomEditor(typeof(TextDetailBinder))]
    public class TextDetailBinderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            var useDefaultStringProp = serializedObject.FindProperty("_useDefaultString");
            EditorGUILayout.PropertyField(useDefaultStringProp);

            if (useDefaultStringProp.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultString"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultStringObserver"));
            }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_stringArgumentObservers"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}