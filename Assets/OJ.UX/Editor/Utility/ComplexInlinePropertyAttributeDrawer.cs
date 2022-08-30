using OJ.UX.Runtime.Utility;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Utility
{
    [CustomPropertyDrawer(typeof(ComplexInlinePropertyAttribute))]
    public class ComplexInlinePropertyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ComplexInlinePropertyAttribute)attribute;
            if (attr.PropertyFields == null)
                return;

            var size = attr.PropertyFields.Length;
            var pos = new Rect(position.x, position.y, position.width / size, position.height);
            foreach (var propString in attr.PropertyFields)
            {
                var includeName = false;
                var finalPropString = propString;
                if (propString.StartsWith("@"))
                {
                    finalPropString = propString.Substring(1);
                    includeName = true;
                }
                
                var prop = property.FindPropertyRelative(finalPropString);
                if (prop != null)
                {
                    EditorGUI.PropertyField(pos, prop, includeName ? new GUIContent(finalPropString) : GUIContent.none, true);
                    pos.x += pos.width + 2;
                }
            }
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}