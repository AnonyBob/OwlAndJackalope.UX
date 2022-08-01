using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomPropertyDrawer(typeof(StringNullOrEmptyComparison))]
    public class StringNullOrEmptyComparisonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("_compare"), GUIContent.none);
        }
    }
}