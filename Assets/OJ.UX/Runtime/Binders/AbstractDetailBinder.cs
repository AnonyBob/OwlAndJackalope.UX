using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class AbstractDetailBinder : MonoBehaviour, IDetailBinder
    {
        public virtual bool RespondToNameChange(ReferenceModule changingModule, string originalName, string newName)
        {
            var didChange = false;
            var observerType = typeof(Observer);
            var listOfObserverType = typeof(List<Observer>);
            
            var detailNameFieldInfo = observerType.GetField("_detailName", BindingFlags.Instance | BindingFlags.NonPublic);
            var referenceModuleFieldInfo = observerType.GetField("_referenceModule", BindingFlags.Instance | BindingFlags.NonPublic);
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType.IsSubclassOf(observerType))
                {
                    var observer = field.GetValue(this);
                    CheckAndUpdateObserverDetailName(observer, changingModule, originalName,
                        newName, referenceModuleFieldInfo, detailNameFieldInfo, ref didChange);
                }
                else if (field.FieldType == listOfObserverType)
                {
                    var array = (List<Observer>)field.GetValue(this);
                    if (array != null)
                    {
                        foreach (var observer in array)
                        {
                            CheckAndUpdateObserverDetailName(observer, changingModule, originalName,
                                newName, referenceModuleFieldInfo, detailNameFieldInfo, ref didChange);
                        }
                    }
                }
                else if (field.FieldType.IsArray && field.FieldType.GetElementType() == observerType)
                {
                    var array = (Observer[])field.GetValue(this);
                    if (array != null)
                    {
                        foreach (var observer in array)
                        {
                            CheckAndUpdateObserverDetailName(observer, changingModule, originalName,
                                newName, referenceModuleFieldInfo, detailNameFieldInfo, ref didChange);
                        }
                    }
                }
            }

            if (didChange)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }

            return didChange;
        }

        private void CheckAndUpdateObserverDetailName(object observer, ReferenceModule changingModule, string oldName, string newName, 
            FieldInfo referenceModuleFieldInfo, FieldInfo detailNameFieldInfo, ref bool didChange)
        {
            var referenceModule = (ReferenceModule)referenceModuleFieldInfo?.GetValue(observer);
            if (referenceModule != changingModule)
            {
                return;
            }
                    
            var detailName = (string)detailNameFieldInfo?.GetValue(observer);
            if (detailName == oldName)
            {
                detailNameFieldInfo?.SetValue(observer, newName);
                didChange = true;
            }
        }
    }
}