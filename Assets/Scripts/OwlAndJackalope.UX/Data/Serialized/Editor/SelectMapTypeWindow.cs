using System;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    public class SelectMapTypeWindow : EditorWindow
    {
        private DetailType _keyType;
        private DetailType _valueType;
        private Action<DetailType, DetailType> _selectAction;

        public static void Open(Rect position, Action<DetailType, DetailType> selectAction)
        {
            var screenPoint = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = screenPoint.x;
            position.y = screenPoint.y + 15;
            position.width = 300;
            position.height = EditorGUIUtility.singleLineHeight * 4;
            
            var window = CreateInstance<SelectMapTypeWindow>();
            window.position = position;
            window._selectAction = selectAction;
            window.ShowPopup();
        }

        private void OnLostFocus()
        {
            Close();
        }

        private void OnGUI()
        {
            _keyType = (DetailType)EditorGUILayout.EnumPopup("Key Type", _keyType);
            _valueType = (DetailType)EditorGUILayout.EnumPopup("Value Type", _valueType);

            if (GUILayout.Button("Select"))
            {
                _selectAction?.Invoke(_keyType, _valueType);
                Close();
            }
        }
    }
}