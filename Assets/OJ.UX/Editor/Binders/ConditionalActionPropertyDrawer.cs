using OJ.UX.Editor.Utility;
using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomPropertyDrawer(typeof(ConditionalAction<>), true)]
    public class ConditionalActionPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty conditionalAction, GUIContent label)
        {
            var isOpenProp = conditionalAction.FindPropertyRelative("_isOpen");
            if (!isOpenProp.boolValue)
            {
                return base.GetPropertyHeight(conditionalAction, label);
            }
            
            var headerHeight = EditorGUIUtility.singleLineHeight + 5;
            var groupHeight = 0f;
            var conditionGroupsProp = conditionalAction.FindPropertyRelative("_conditionGroups");
            for (var groupIndex = 0; groupIndex < conditionGroupsProp.arraySize; ++groupIndex)
            {
                var conditionGroup = conditionGroupsProp.GetArrayElementAtIndex(groupIndex);
                var conditions = conditionGroup.FindPropertyRelative("_conditions");
                if (conditions.arraySize == 0)
                {
                    groupHeight += EditorGUIUtility.singleLineHeight + 4;
                }
                else
                {
                    var conditionHeight = (EditorGUIUtility.singleLineHeight * 2) + 6;
                    var boxHeight = (conditions.arraySize * conditionHeight) + 6 + ((conditions.arraySize - 1) * 2);
                    groupHeight += boxHeight + 4;
                }

                var orHeight = EditorGUIUtility.singleLineHeight + 2;
                groupHeight += orHeight;
            }

            if (conditionGroupsProp.arraySize == 0)
            {
                groupHeight += EditorGUIUtility.singleLineHeight + 2;
            }
            
            return headerHeight + groupHeight + GetActionWithConditionalHeight(conditionalAction) + 10;
        }
        
        public override void OnGUI(Rect position, SerializedProperty conditionalAction, GUIContent label)
        {
            var pos = new Rect(position.x, position.y, position.width, position.height);
            if (DrawHeader(ref pos, conditionalAction))
            {
                pos.y += 5;
                DrawConditionGroups(ref pos, conditionalAction);
            }
        }
        
        private bool DrawHeader(ref Rect pos, SerializedProperty conditionalAction)
        {
            var isOpenProp = conditionalAction.FindPropertyRelative("_isOpen");
            var actionDescriptionProp = conditionalAction.FindPropertyRelative("_actionDescription");
            var previousColor = GUI.backgroundColor;
            var content = string.Empty;
            if (Application.isPlaying)
            {
                var isMet = CheckConditionIsMet(conditionalAction.serializedObject, 0);
                GUI.backgroundColor = isMet ? Color.green : Color.red;
                content = isMet ? "Active" : "Inactive";
            }

            var foldoutPos = new Rect(pos);
            foldoutPos.x = foldoutPos.x + 10;
            foldoutPos.width = 20;
            foldoutPos.height = EditorGUIUtility.singleLineHeight;
            isOpenProp.boolValue = EditorGUI.Foldout(foldoutPos, isOpenProp.boolValue, content, true);
            GUI.backgroundColor = previousColor;

            var namePos = new Rect(pos);
            namePos.x = namePos.x + foldoutPos.x + foldoutPos.width;
            namePos.y += 2;
            namePos.height =  EditorGUIUtility.singleLineHeight;
            namePos.width = pos.width - foldoutPos.x - foldoutPos.width;
            EditorGUI.PropertyField(namePos, actionDescriptionProp, GUIContent.none);

            pos.y = pos.y + namePos.height + 2;
            pos.height = pos.height - namePos.height - 2;
            
            return isOpenProp.boolValue;
        }

        private void DrawConditionGroups(ref Rect pos, SerializedProperty conditionalAction)
        {
            var conditionGroupsProp = conditionalAction.FindPropertyRelative("_conditionGroups");
            for (var groupIndex = 0; groupIndex < conditionGroupsProp.arraySize; ++groupIndex)
            {
                var conditionGroup = conditionGroupsProp.GetArrayElementAtIndex(groupIndex);
                var conditions = conditionGroup.FindPropertyRelative("_conditions");

                var boxPos = new Rect(pos);
                boxPos.x = pos.x + 5;
                boxPos.width = pos.width - 10;
                if (conditions.arraySize == 0)
                {
                    boxPos.height = EditorGUIUtility.singleLineHeight + 4;
                    GUI.Box(boxPos, GUIContent.none, EditorStyles.textArea);

                    var previousColor = GUI.backgroundColor;
                    var buttonPos = new Rect(boxPos);
                    buttonPos.width = 50;
                    buttonPos.x = boxPos.x + (boxPos.width - 100) / 2;
                    buttonPos.y = boxPos.y + 2;
                    buttonPos.height = EditorGUIUtility.singleLineHeight;

                    GUI.backgroundColor = Color.green;
                    if (GUI.Button(buttonPos, "+", EditorStyles.miniButton))
                    {
                        AddAnotherCondition(conditions, buttonPos);
                    }
                    
                    buttonPos.x = buttonPos.x + 50;
                    GUI.backgroundColor = Color.red;
                    if (GUI.Button(buttonPos, "-", EditorStyles.miniButton))
                    {
                        GUI.backgroundColor = previousColor;
                        conditionGroupsProp.DeleteArrayElementAtIndex(groupIndex);
                        groupIndex--;
                    }
                    
                    GUI.backgroundColor = previousColor;
                }
                else
                {
                    var deleteConditionGroup = false;
                    var conditionHeight = (EditorGUIUtility.singleLineHeight * 2) + 6;
                    boxPos.height = (conditions.arraySize * conditionHeight) + 6 + ((conditions.arraySize - 1) * 2);
                    GUI.Box(boxPos, GUIContent.none, EditorStyles.textArea);

                    var conditionPos = new Rect(boxPos.x + 10, boxPos.y + 2, boxPos.width - 20, (EditorGUIUtility.singleLineHeight + 4) * 2);
                    for (var conditionIndex = 0; conditionIndex < conditions.arraySize; ++conditionIndex)
                    {
                        if (DrawCondition(conditionPos, conditions, conditionIndex, groupIndex))
                        {
                            conditions.DeleteArrayElementAtIndex(conditionIndex);
                            conditionIndex--;
                        
                            if (conditions.arraySize == 0)
                            {
                                deleteConditionGroup = true;
                            }
                        }

                        conditionPos.y += conditionHeight + 2;
                    }

                    if (deleteConditionGroup)
                    {
                        conditionGroupsProp.DeleteArrayElementAtIndex(groupIndex);
                        groupIndex--;
                    }
                }
                
                pos.y += boxPos.height + 2;
                var orPos = new Rect(pos.x + ((pos.width - 50) / 2), pos.y, 50, EditorGUIUtility.singleLineHeight);
                
                if (groupIndex == conditionGroupsProp.arraySize - 1)
                {
                    if (GUI.Button(orPos, "or", EditorStyles.miniButton))
                    {
                        AddAnotherConditionGroup(orPos, conditionGroupsProp);
                    }
                }
                else
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleCenter;
                    
                    EditorGUI.LabelField(orPos, "or", style);
                }

                pos.y += orPos.height + 2;
            }

            if (conditionGroupsProp.arraySize == 0)
            {
                var orPos = new Rect(pos.x + ((pos.width - 50) / 2), pos.y, 50, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(orPos, "or", EditorStyles.miniButton))
                {
                    AddAnotherConditionGroup(orPos, conditionGroupsProp);
                }

                pos.y += orPos.height + 2;
            }

            var actionPos = new Rect(pos.x, pos.y, pos.width, GetActionWithConditionalHeight(conditionalAction));
            DrawActionWithinConditional(actionPos, conditionalAction);
        }
        
        private bool DrawCondition(Rect pos, SerializedProperty conditions, int conditionIndex, int groupIndex)
        {
            var delete = false;
            var condition = conditions.GetArrayElementAtIndex(conditionIndex);
            var comparison = condition.FindPropertyRelative("_comparison");
            GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);

            var observerPos = new Rect(pos.x + 5, pos.y + 2, pos.width - 73, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(observerPos, condition.FindPropertyRelative("_observer"), GUIContent.none);

            var greenButPos = new Rect(observerPos.x + observerPos.width + 2, observerPos.y, 30, observerPos.height);
            using (new BackgroundColorScope(Color.green))
            {
                if (GUI.Button(greenButPos, "+", EditorStyles.miniButton))
                {
                    AddAnotherCondition(conditions, greenButPos);
                }
            }
            
            var redButPos = new Rect(greenButPos.x + greenButPos.width + 2, observerPos.y, 30, observerPos.height);
            using (new BackgroundColorScope(Color.red))
            {
                delete = GUI.Button(redButPos, "-", EditorStyles.miniButton);
            }

            var comparisonPos = new Rect(observerPos.x, observerPos.y + observerPos.height + 2, pos.width - 8, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(comparisonPos, comparison, GUIContent.none, true);

            return delete;
        }

        private void AddAnotherConditionGroup(Rect orRect, SerializedProperty conditionGroupsProp)
        {
            conditionGroupsProp.arraySize++;
            var newGroup = conditionGroupsProp.GetArrayElementAtIndex(conditionGroupsProp.arraySize - 1);
            var conditions = newGroup.FindPropertyRelative("_conditions");

            //Get the reference module from the previous group. We will likely want to assign it in the next 
            //group as well.
            Object previousReferenceModule = null;
            if (conditions.arraySize > 0)
            {
                var firstCondition = conditions.GetArrayElementAtIndex(0);
                var observer = firstCondition.FindPropertyRelative("_observer");
                if (observer != null)
                {
                    var referenceModule = observer.FindPropertyRelative("_referenceModule");
                    previousReferenceModule = referenceModule.objectReferenceValue;
                }
            }
                
            conditions.arraySize = 0;
            AddAnotherCondition(conditions, orRect, previousReferenceModule);
        }

        private void AddAnotherCondition(SerializedProperty conditions, Rect? lastRect, Object previousReferenceModule = null)
        {
            if (lastRect.HasValue)
            {
                var menu = ConditionsUtility.GetInitialConditionMenu(conditions, previousReferenceModule);
                menu.DropDown(lastRect.Value);
            }
        }

        private bool CheckConditionIsMet(SerializedObject conditionalBinder, int conditionalActionIndex)
        {
            var type = conditionalBinder.targetObject.GetType();
            var method = type.GetMethod("IsConditionalActionActive");
            if (method != null)
            {
                var result = method.Invoke(conditionalBinder.targetObject, new object[] { conditionalActionIndex });
                return (bool)result;
            }
            return false;
        }

        protected virtual float GetActionWithConditionalHeight(SerializedProperty conditionalAction)
        {
            var action = conditionalAction.FindPropertyRelative("_action");
            return EditorGUI.GetPropertyHeight(action, true);
        }
        
        protected virtual void DrawActionWithinConditional(Rect pos, SerializedProperty conditionalAction)
        {
            EditorGUI.PropertyField(pos, conditionalAction.FindPropertyRelative("_action"), true);
        }
    }
}