using System;
using OwlAndJackalope.UX.Modules;
using UnityEditor;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers
{
    public delegate void UpdateSerializedValue(SerializedProperty property, BasePropertyData propertyData);
    
    public class BasePropertyData
    {
        public IReference RuntimeReference { get; set; }
        public long RuntimeReferenceVersion { get; set; }
        public IDetail RuntimeDetail { get; set; }
        public long RuntimeDetailVersion { get; set; }

        public void HandleRuntimeDetailChanged(SerializedProperty property, UpdateSerializedValue func)
        {
            if (Application.isPlaying)
            {
                if (RuntimeDetail == null || RuntimeReference == null)
                {
                    SetReferenceAndDetail(property);
                }
                else if (RuntimeReference?.Version != RuntimeReferenceVersion)
                {
                    SetReferenceAndDetail(property);
                }
                else if (RuntimeDetail != null && RuntimeDetail.Version != RuntimeDetailVersion)
                {
                    func(property, this);
                    RuntimeDetailVersion = RuntimeDetail.Version;
                }
            }
        }

        private void SetReferenceAndDetail(SerializedProperty property)
        {
            var module = property.serializedObject.targetObject as ReferenceModule;
            if (module != null)
            {
                var name = property.FindPropertyRelative(SharedDrawers.NameString).stringValue;
                RuntimeDetail = module.Reference.GetDetail(name);
                RuntimeReference = module.Reference;
                RuntimeReferenceVersion = module.Reference.Version;
                RuntimeDetailVersion = -1;
            }
        }
    }
}