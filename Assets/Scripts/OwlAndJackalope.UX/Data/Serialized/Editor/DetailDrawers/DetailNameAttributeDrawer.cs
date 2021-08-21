using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OwlAndJackalope.UX.Modules;
using OwlAndJackalope.UX.Observers;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers
{
    [CustomPropertyDrawer(typeof(DetailNameAttribute))]
    public class DetailNameAttributeDrawer : PropertyDrawer
    {
        public class PropertyData
        {
            public ReferenceModule Module;
            public DetailNameAttribute Attribute;
        }
        protected readonly Dictionary<string, PropertyData> _data = new Dictionary<string, PropertyData>();

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            if (!_data.TryGetValue(property.propertyPath, out var propertyData))
            {
                propertyData = new PropertyData();
                propertyData.Attribute = (DetailNameAttribute) attribute;
                _data[property.propertyPath] = propertyData;
            }

            if (propertyData.Module == null)
            {
                var containingObject = (property.serializedObject.targetObject as MonoBehaviour);
                if (containingObject != null)
                {
                    propertyData.Module = containingObject.GetComponentInParent<ReferenceModule>();
                }
            }

            if (propertyData.Module != null)
            {
                var options = GetOptions(propertyData);
                var currentIndex = Math.Max(0, Array.IndexOf(options, property.stringValue));
                var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options);
                if (selectedIndex >= 0 && selectedIndex < options.Length)
                {
                    property.stringValue = options[selectedIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private string[] GetOptions(PropertyData data)
        {
            var details = data.Module.SerializedReference.GetDetails(data.Attribute.AcceptableTypes);
            return details.Select(x => x.Name).ToArray();
        }
    }
}