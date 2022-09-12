using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(SerializedReference))]
    public class SerializedReferencePropertyDrawer : PropertyDrawer
    {
        private ReorderableList _list;
        private GenericMenu _menu;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_list == null)
            {
                var detailsProp = property.FindPropertyRelative(nameof(SerializedReference.Details));
                _menu = SerializedEditorReferenceUtility.CreateSelectionMenu(detailsProp);
                _list = SerializedEditorReferenceUtility.CreateDetailList(label, detailsProp, _menu);
            }
            
            _list.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_list != null)
            {
                return _list.GetHeight();
            }

            return base.GetPropertyHeight(property, label);
        }
    }
}