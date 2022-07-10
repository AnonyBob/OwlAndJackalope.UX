using System.Reflection;
using OJ.UX.Runtime.Binders;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(ISerializedValueDetail<>), true)]
    public class SerializedValueDetailPropertyDrawer : PropertyDrawer
    {
        private const string VALUE_PROP = "Value";
        private const float BUFFER = 2;

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

                startingPos = position.x + 22;
                startingWidth = 22;
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
            var nameValue = EditorGUI.TextField(namePos, nameProp.stringValue);
            if (nameValue != nameProp.stringValue && NameValueIsNotDuplicate(nameValue, property))
            {
                var previousNameValue = nameProp.stringValue;
                nameProp.stringValue = nameValue;
                UpdateObservingBinders(property, previousNameValue, nameValue);
            }
            
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
        
        private bool NameValueIsNotDuplicate(string newName, SerializedProperty property)
        {
            if (property.serializedObject.targetObject is ReferenceModule module)
            {
                return module.Editor_CheckName(newName);
            }

            return true;
        }
        
        private void UpdateObservingBinders(SerializedProperty property, string originalName, string newName)
        {
            if (property.serializedObject.targetObject is ReferenceModule module)
            {
                var detailBinders = Object.FindObjectsOfType<AbstractDetailBinder>();
                foreach (var detailBinder in detailBinders)
                {
                    detailBinder.RespondToNameChange(module, originalName, newName);
                }
            }
        }
    }
}