using System.Reflection;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(ISerializedValueDetail<>), true)]
    public class SerializedValueDetailPropertyDrawer : PropertyDrawer
    {
        private const string VALUE_PROP = "Value";
        private const float BUFFER = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var startingPos = position.x;
            var startingWidth = 0;
            
            var detail = property.managedReferenceValue as ISerializedDetail;
            if (Application.isPlaying)
            {
                detail?.RespondToChangesInRuntimeDetail();

                var provided = detail?.IsRuntimeDetailProvided() ?? false;
                var statusPos = new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight);

                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = provided ? Color.green : Color.blue;
                EditorGUI.LabelField(statusPos, new GUIContent(provided ? "P" : "L", 
                    provided ? "Detail is provided by external reference." : "Detail is local to this container."),
                    "button");

                GUI.backgroundColor = originalColor;

                startingPos = 30;
                startingWidth = 30;
            }

            var nameProp = property.FindPropertyRelative(nameof(AbstractSerializedDetail.Name));
            var valueProp = property.FindPropertyRelative(VALUE_PROP);
        
            var typePos = new Rect(startingPos, position.y, position.width * 0.2f, EditorGUIUtility.singleLineHeight);
            var serializedDetail = property.managedReferenceValue as ISerializedDetail;
            var typeString = serializedDetail?.GetType().GetCustomAttribute<SerializedDetailDisplayAttribute>()?.DisplayName ??
                serializedDetail?.GetValueType().Name;
            EditorGUI.LabelField(typePos, typeString, EditorStyles.helpBox);   
            
            var previousEnableState = GUI.enabled;
            GUI.enabled = !Application.isPlaying;
            
            var namePos = new Rect(startingPos + typePos.width + BUFFER, position.y, position.width * 0.3f - BUFFER, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(namePos, nameProp, GUIContent.none);
            
            GUI.enabled = previousEnableState;
        
            var xPos = namePos.x + namePos.width + BUFFER;
            var width = position.width - (namePos.width + typePos.width + startingWidth) - (BUFFER * 2);
            var valuePos = new Rect(xPos, position.y, width, EditorGUIUtility.singleLineHeight);
            if (valueProp == null)
            {
                EditorGUI.LabelField(valuePos, $"Can't find {VALUE_PROP} property!");
            }
            else
            {
                previousEnableState = GUI.enabled;
                GUI.enabled = !Application.isPlaying || (detail?.CanMutateRuntimeDetail() ?? false);
                
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(valuePos, valueProp, GUIContent.none);
        
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                    detail?.ForceUpdateRuntimeDetail();
                }

                GUI.enabled = previousEnableState;
            }
        }
    }
}