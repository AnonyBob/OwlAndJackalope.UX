using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomPropertyDrawer(typeof(IConditionComparison), true)]
    public class ConditionValueComparisonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pos = new Rect(position);
            pos.width = pos.width * 0.5f - 2f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("_comparisonType"), GUIContent.none);

            pos.x = pos.x + pos.width + 2f;
            EditorGUI.PropertyField(pos, property.FindPropertyRelative("_comparisonValue"), GUIContent.none);
        }
    }
}