using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Editor.DetailDrawers
{
    public class SelectMapTypeWindow : EditorWindow
    {
        private int _keySelectionIndex;
        private int _valueSelectionIndex;
        private Action<FullDetailType, FullDetailType> _selectAction;

        public struct FullDetailType
        {
            public DetailType MainType;
            public Type EnumType;

            public override string ToString()
            {
                return EnumType?.Name ?? MainType.ToString();
            }
        }
        
        public static void Open(Rect position, Action<FullDetailType, FullDetailType> selectAction)
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
            var options = GetOptions().ToArray();
            _keySelectionIndex = EditorGUILayout.Popup("Key Type", _keySelectionIndex, options);
            _valueSelectionIndex = EditorGUILayout.Popup("Value Type", _valueSelectionIndex, options);

            if (GUILayout.Button("Select"))
            {
                _selectAction?.Invoke(ConstructFullDetail(options, _keySelectionIndex), 
                    ConstructFullDetail(options, _valueSelectionIndex));
                Close();
            }
        }

        private FullDetailType ConstructFullDetail(string[] options, int index)
        {
            if (index < 0 || index >= options.Length)
            {
                return new FullDetailType();
            }
            
            var selected = options[index];
            if (!selected.Contains("/"))
            {
                return new FullDetailType()
                {
                    MainType = (DetailType)Enum.Parse(typeof(DetailType), selected)
                };
            }
            
            return new FullDetailType()
            {
                MainType = DetailType.Enum,
                EnumType = SerializedDetailEnumCache.GetEnumType(selected.Substring(selected.IndexOf('/') + 1))
            };
        }
        
        private IEnumerable<string> GetOptions()
        {
            var detailType = typeof(DetailType);
            var enumTypeString = DetailType.Enum.ToString();
            foreach (var typeString in Enum.GetNames(detailType))
            {
                if (typeString != enumTypeString)
                {
                    yield return typeString;
                }
                else
                {
                    foreach (var enumType in SerializedDetailEnumCache.EnumTypes)
                    {
                        yield return $"{enumTypeString}/{enumType.Name}";
                    }
                }
            }
        }
    }
}