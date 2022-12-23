using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OJ.UX.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor
{
    public static partial class OJEditorUtility
    {
        private static readonly Dictionary<string, Rect> _lastRects = new Dictionary<string, Rect>();

        public static bool Button(string text, GUIStyle style = null)
        {
            return GUILayout.Button(text, style ?? GUI.skin.button);
        }
        
        public static bool Button(Texture text, GUIStyle style = null)
        {
            return GUILayout.Button(text, style ?? GUI.skin.button);
        }
        
        public static bool Button(string text, Color color, GUIStyle style = null)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            
            var pressed =  GUILayout.Button(text, style ?? GUI.skin.button);
            GUI.backgroundColor = previousColor;

            return pressed;
        }
        
        public static bool Button(Texture text, Color color, GUIStyle style = null)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            
            var pressed =  GUILayout.Button(text, style ?? GUI.skin.button);
            GUI.backgroundColor = previousColor;

            return pressed;
        }
        
        public static bool Button(string text, Color color, float width, GUIStyle style = null)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            
            var pressed =  GUILayout.Button(text, style ?? GUI.skin.button, GUILayout.Width(width));
            GUI.backgroundColor = previousColor;

            return pressed;
        }
        
        public static bool Button(Texture text, Color color, float width, GUIStyle style = null)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            
            var pressed =  GUILayout.Button(text, style ?? GUI.skin.button, GUILayout.Width(width));
            GUI.backgroundColor = previousColor;

            return pressed;
        }
        
        public static bool Button(string text, Color color, float width, float height, GUIStyle style = null)
        {
            var previousColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            
            var pressed =  GUILayout.Button(text, style ?? GUI.skin.button, GUILayout.Width(width), GUILayout.Height(height));
            GUI.backgroundColor = previousColor;

            return pressed;
        }
        
        public static bool CenteredButton(string text, Color color, float width, GUIStyle style = null, string rectId = null)
        {
            var pressed = false;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var previousColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
                
                pressed =  GUILayout.Button(text, style ?? GUI.skin.button,GUILayout.Width(width));
                if (rectId != null)
                {
                    SetLastRect(rectId);
                }
                
                GUI.backgroundColor = previousColor;
                GUILayout.FlexibleSpace();
            }
            
            return pressed;
        }
        
        public static bool CenteredButton(Texture text, Color color, float width, float height = 0f, GUIStyle style = null, string rectId = null)
        {
            var pressed = false;
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var previousColor = GUI.backgroundColor;
                GUI.backgroundColor = color;

                if (height > 0)
                {
                    pressed = GUILayout.Button(text, style ?? GUI.skin.button,GUILayout.Width(width), GUILayout.Height(height));  
                }
                else
                {
                    pressed =  GUILayout.Button(text, style ?? GUI.skin.button,GUILayout.Width(width));    
                }
                
                if (rectId != null)
                {
                    SetLastRect(rectId);
                }
                
                GUI.backgroundColor = previousColor;
                GUILayout.FlexibleSpace();
            }
            
            return pressed;
        }

        public static void SetLastRect(string rectIdentifier)
        {
            if (Event.current.type == EventType.Repaint)
            {
                _lastRects[rectIdentifier] = GUILayoutUtility.GetLastRect();
            }
        }

        public static Rect? GetLastRect(string rectIdentifier)
        {
            if (_lastRects.TryGetValue(rectIdentifier, out var rect))
                return rect;

            return null;
        }

        public static void DrawLine(Color color, float thickness = 2, float padding = 10)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness + padding));
            rect.height = thickness;
            rect.y += padding / 2;
            rect.x += padding / 2;

            rect.width -= padding;
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawTimeSpanFromLong(Rect valuePos, SerializedProperty valueProp)
        {
            var previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 12f;
            
            var timeSpan = TimeSpan.FromTicks(valueProp.longValue);
            var pos = new Rect(valuePos.x, valuePos.y, valuePos.width / 4 - 2, valuePos.height);

            var days = EditorGUI.IntField(pos, "D", timeSpan.Days);
            pos.x += pos.width + 2;
            
            var hours = EditorGUI.IntField(pos, "H", timeSpan.Hours);
            pos.x += pos.width + 2;
            
            var minutes = EditorGUI.IntField(pos, "M", timeSpan.Minutes);
            pos.x += pos.width + 2;
            
            var seconds = EditorGUI.IntField(pos, "S", timeSpan.Seconds);
            pos.x += pos.width + 2;

            timeSpan = new TimeSpan(days, hours, minutes, seconds);
            valueProp.longValue = timeSpan.Ticks;

            EditorGUIUtility.labelWidth = previousWidth;
        }

        public static void DrawDateTimeFromLong(Rect valuePos, SerializedProperty valueProp)
        {
            var dateTime = new DateTime(valueProp.longValue, DateTimeKind.Utc);
            var pos = new Rect(valuePos);
            pos.width = valuePos.width * .75f;

            var changedDate = false;
            EditorGUI.BeginChangeCheck();
            var originalDateString = dateTime.ToString("G");
            var dateString = EditorGUI.TextField(pos, GUIContent.none, originalDateString);
            if (EditorGUI.EndChangeCheck() && originalDateString != dateString)
            {
                if (DateTime.TryParse(dateString, out var date))
                {
                    valueProp.longValue = date.Ticks;
                }

                changedDate = true;
            }

            pos.x = pos.x + pos.width + 2;
            pos.width = valuePos.width - pos.width - 2;
            var isDark = EditorGUIUtility.isProSkin;
            var stopWatchTex = Resources.Load<Texture>(isDark ? "stopwatch" : "stopwatch_black");
            if (GUI.Button(pos, stopWatchTex) && !changedDate)
            {
                PopupWindow.Show(valuePos, new DateTimePickerWindow(valueProp));
            }
        }
        
        public static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                    {
                        // reached the end
                        break;
                    }
                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
    }
}