using System.Reflection;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(ISerializedValueDetail<>), true)]
    public class SerializedValueDetailPropertyDrawer : PropertyDrawer
    {
        private const string VALUE_PROP = "Value";
        private const float BUFFER = 5;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                var detail = property.managedReferenceValue as ISerializedDetail;
                detail?.RespondToChangesInRuntimeDetail();
            }
            
            var nameProp = property.FindPropertyRelative(nameof(AbstractSerializedDetail.Name));
            var valueProp = property.FindPropertyRelative(VALUE_PROP);

            var typePos = new Rect(position.x, position.y, position.width * 0.2f, EditorGUIUtility.singleLineHeight);
            var serializedDetail = property.managedReferenceValue as ISerializedDetail;
            var typeString = serializedDetail?.GetType().GetCustomAttribute<SerializedDetailDisplayAttribute>()?.DisplayName ??
                serializedDetail?.GetValueType().Name;
            EditorGUI.LabelField(typePos, typeString, EditorStyles.helpBox);   
            
            var previousEnableState = GUI.enabled;
            GUI.enabled = !Application.isPlaying;
            
            var namePos = new Rect(position.x + typePos.width + BUFFER, position.y, position.width * 0.3f - BUFFER, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(namePos, nameProp, GUIContent.none);
            
            GUI.enabled = previousEnableState;

            var xPos = namePos.x + namePos.width + BUFFER;
            var width = position.width - (namePos.width + typePos.width) - BUFFER - BUFFER;
            var valuePos = new Rect(xPos, position.y, width, EditorGUIUtility.singleLineHeight);
            if (valueProp == null)
            {
                EditorGUI.LabelField(valuePos, $"Can't find {VALUE_PROP} property!");
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(valuePos, valueProp, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                    
                    var detail = property.managedReferenceValue as ISerializedDetail;
                    detail?.ForceUpdateRuntimeDetail();
                }
            }
        }
    }
}