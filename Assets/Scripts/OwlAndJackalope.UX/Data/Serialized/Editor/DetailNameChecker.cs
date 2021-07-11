using OwlAndJackalope.UX.Modules;
using UnityEditor;

namespace OwlAndJackalope.UX.Data.Serialized.Editor
{
    public class DetailNameChecker
    {
        private readonly string NameString = "_name";
        private readonly string ReferenceTemplatePath;
        private readonly string ExperiencePath;
        
        public DetailNameChecker(string referenceTemplatePath, string experiencePath)
        {
            ReferenceTemplatePath = referenceTemplatePath;
            ExperiencePath = experiencePath;
        }
        
        public string CheckName(string previousName, string newName, SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            if(targetObject is ReferenceTemplate)
            {
                var details = property.serializedObject.FindProperty(ReferenceTemplatePath);
                for (var i = 0; i < details.arraySize; ++i)
                {
                    var detailProp = details.GetArrayElementAtIndex(i);
                    if (detailProp.FindPropertyRelative(NameString).stringValue == newName)
                    {
                        return previousName;
                    }
                }
            }
            else if (targetObject is Experience experience)
            {
                var details = property.serializedObject.FindProperty(ExperiencePath);
                for (var i = 0; i < details.arraySize; ++i)
                {
                    var detailProp = details.GetArrayElementAtIndex(i);
                    if (detailProp.FindPropertyRelative(NameString).stringValue == newName)
                    {
                        return previousName;
                    }
                }

                if (previousName != newName)
                {
                    experience.HandleDetailNameChange(previousName, newName);    
                }
                
            }
            else if(CheckNameCustom(newName, property))
            {
                return previousName;
            }
            
            return newName;
        }

        protected virtual bool CheckNameCustom(string newName, SerializedProperty property)
        {
            //OVERRIDE TO ADD ADDITIONAL NAME CHECKS.
            return false;
        }
    }
}