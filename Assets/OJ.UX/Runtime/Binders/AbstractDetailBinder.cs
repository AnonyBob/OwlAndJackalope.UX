using System.Reflection;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class AbstractDetailBinder : MonoBehaviour, IDetailBinder
    {
        public void RespondToNameChange(ReferenceModule changingModule, string originalName, string newName)
        {
            var didChange = false;
            var observerType = typeof(Observer);
            var detailNameFieldInfo = observerType.GetField("_detailName", BindingFlags.Instance | BindingFlags.NonPublic);
            var referenceModuleFieldInfo = observerType.GetField("_referenceModule", BindingFlags.Instance | BindingFlags.NonPublic);
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                if (field.FieldType.IsSubclassOf(observerType))
                {
                    var observer = field.GetValue(this);
                    var referenceModule = (ReferenceModule)referenceModuleFieldInfo?.GetValue(observer);
                    if (referenceModule != changingModule)
                    {
                        continue;
                    }
                    
                    var detailName = (string)detailNameFieldInfo?.GetValue(observer);
                    if (detailName == originalName)
                    {
                        detailNameFieldInfo?.SetValue(observer, newName);
                        didChange = true;
                    }
                }
            }

            if (didChange)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}