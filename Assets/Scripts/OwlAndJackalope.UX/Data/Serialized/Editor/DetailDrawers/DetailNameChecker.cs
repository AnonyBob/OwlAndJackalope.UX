using OwlAndJackalope.UX.Modules;
using UnityEditor;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.DetailDrawers
{
    public class DetailNameChecker
    {
        private readonly string NameString = "_name";
        private readonly string ReferenceTemplatePath;
        private readonly string ReferenceModulePath;

        public DetailNameChecker(string referenceTemplatePath, string referenceModulePath)
        {
            ReferenceTemplatePath = referenceTemplatePath;
            ReferenceModulePath = referenceModulePath;
        }

        public string CheckName(string previousName, string newName, SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            
            if (targetObject is ReferenceTemplate)
            {
                var reference = property.serializedObject.FindProperty(ReferenceTemplatePath);
                if (!IsNameValid(reference, newName))
                {
                    return previousName;
                }
            }
            else if (targetObject is ReferenceModule module)
            {
                var reference = property.serializedObject.FindProperty(ReferenceModulePath);
                if (!IsNameValid(reference, newName))
                {
                    return previousName;
                }
                
                if (previousName != newName)
                {
                    //UpdateConditions(property.serializedObject.FindProperty(SharedDrawers.ExperienceStatesPath), previousName, newName);
                    module.HandleDetailNameChange(previousName, newName);
                }
            }

            return newName;
        }

        private bool IsNameValid(SerializedProperty referenceProp, string newName)
        {
            return IsNameValid(referenceProp, SharedDrawers.ReferenceDetailsString, newName)
                   && IsNameValid(referenceProp, SharedDrawers.ReferenceCollectionDetailsString, newName)
                   && IsNameValid(referenceProp, SharedDrawers.ReferenceMapDetailsString, newName);
        }
        
        private bool IsNameValid(SerializedProperty referenceProp, string detailsPath, string newName)
        {
            var details = referenceProp.FindPropertyRelative(detailsPath);
            for (var i = 0; i < details.arraySize; ++i)
            {
                var detailProp = details.GetArrayElementAtIndex(i);
                if (detailProp.FindPropertyRelative(NameString).stringValue == newName)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateConditions(SerializedProperty states, string previousName, string newName)
        {
            for (var i = 0; i < states.arraySize; ++i)
            {
                var state = states.GetArrayElementAtIndex(i);
                var conditions = state.FindPropertyRelative(SharedDrawers.ConditionsString);
                for (var j = 0; j < conditions.arraySize; ++j)
                {
                    var condition = conditions.GetArrayElementAtIndex(j);
                    var paramOneName = condition.FindPropertyRelative("_parameterOne.Name");
                    if (paramOneName.stringValue == previousName)
                    {
                        paramOneName.stringValue = newName;
                    }
                    
                    var paramTwoName = condition.FindPropertyRelative("_parameterTwo.Name");
                    if (paramTwoName.stringValue == previousName)
                    {
                        paramTwoName.stringValue = newName;
                    }
                }
            }
        }
    }
}