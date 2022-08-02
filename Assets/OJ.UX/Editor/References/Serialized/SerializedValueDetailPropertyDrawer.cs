using System.Reflection;
using OJ.UX.Runtime.Binders;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    [CustomPropertyDrawer(typeof(SerializedValueDetail<>), true)]
    [CustomPropertyDrawer(typeof(SerializedReferenceDetail))]
    [CustomPropertyDrawer(typeof(SerializedReferenceListValueDetail))]
    public class SerializedValueDetailPropertyDrawer : PropertyDrawer
    {
        private const string VALUE_PROP = "Value";
        private const float BUFFER = 5;

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
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
            var isArray = valueProp.isArray && valueProp.propertyType != SerializedPropertyType.String;
        
            var typePos = new Rect(startingPos, position.y, position.width * 0.2f, EditorGUIUtility.singleLineHeight);
            var serializedDetail = property.managedReferenceValue as ISerializedDetail;
            var typeString = serializedDetail?.GetType().GetCustomAttribute<SerializedDetailDisplayAttribute>()?.DisplayName ??
                serializedDetail?.GetValueType().Name;
            
            EditorGUI.LabelField(typePos, typeString, EditorStyles.helpBox);   
            
            var previousEnableState = GUI.enabled;
            GUI.enabled = !Application.isPlaying;

            var nameWidth = position.width * 0.3f - BUFFER;
            var namePos = new Rect(startingPos + typePos.width + BUFFER, position.y, nameWidth, EditorGUIUtility.singleLineHeight);
            var nameValue = EditorGUI.TextField(namePos, nameProp.stringValue);
            if (nameValue != nameProp.stringValue && NameValueIsNotDuplicate(nameValue, property))
            {
                var previousNameValue = nameProp.stringValue;
                nameProp.stringValue = nameValue;
                UpdateObservingBinders(property, previousNameValue, nameValue);
            }
            
            GUI.enabled = previousEnableState;
        
            var xPos = isArray ? startingPos + (typePos.width / 2)  : namePos.x + namePos.width + BUFFER;
            var yPos = isArray ? position.y + EditorGUIUtility.singleLineHeight + BUFFER : position.y;
            var width = isArray ? position.width - (typePos.width / 2 + BUFFER) 
                : position.width - (namePos.width + typePos.width + startingWidth) - (BUFFER * 2);
            
            var valuePos = new Rect(xPos, yPos, width, EditorGUI.GetPropertyHeight(valueProp));
            HandleValue(valuePos, valueProp, property, detail);
        }

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative(VALUE_PROP);
            if (valueProp != null && valueProp.isArray && valueProp.propertyType != SerializedPropertyType.String)
            {
                return EditorGUI.GetPropertyHeight(valueProp, true) + base.GetPropertyHeight(property, label) + BUFFER;
            }

            return base.GetPropertyHeight(property, label);
        }

        protected virtual void DrawValue(Rect valuePos, SerializedProperty valueProp)
        {
            EditorGUI.PropertyField(valuePos, valueProp, GUIContent.none, true);
        }
        
        private void HandleValue(Rect valuePos, SerializedProperty valueProp, SerializedProperty baseProperty, ISerializedDetail detail)
        {
            var previousEnableState = GUI.enabled;
            GUI.enabled = !Application.isPlaying || (detail?.CanMutateRuntimeDetail() ?? false);
                
            EditorGUI.BeginChangeCheck();
            DrawValue(valuePos, valueProp);
        
            if (EditorGUI.EndChangeCheck())
            {
                baseProperty.serializedObject.ApplyModifiedProperties();
                detail?.ForceUpdateRuntimeDetail();
            }

            GUI.enabled = previousEnableState;
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