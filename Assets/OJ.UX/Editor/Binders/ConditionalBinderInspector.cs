using OJ.UX.Runtime.Binders;
using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomEditor(typeof(ConditionalBinder<>), true)]
    public class ConditionalBinderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var conditionalActionsProp = serializedObject.FindProperty("_conditionalActions");
            if (conditionalActionsProp.arraySize == 0)
            {
                if (OJEditorUtility.CenteredButton("Add Conditional Action", GUI.backgroundColor, 150))
                {
                    AddAnotherConditionalAction(conditionalActionsProp);
                }
            }
            else
            {
                for (var i = 0; i < conditionalActionsProp.arraySize; ++i)
                {
                    var delete = DrawConditionalAction(conditionalActionsProp, i);
                    if (delete)
                    {
                        conditionalActionsProp.DeleteArrayElementAtIndex(i);
                        i--;
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void AddAnotherConditionalAction(SerializedProperty conditionalActionsProp)
        {
            conditionalActionsProp.InsertArrayElementAtIndex(conditionalActionsProp.arraySize);
            var conditionalAction = conditionalActionsProp.GetArrayElementAtIndex(conditionalActionsProp.arraySize - 1);
            conditionalAction.FindPropertyRelative("_actionDescription").stringValue = string.Empty;
            conditionalAction.FindPropertyRelative("_conditionGroups").arraySize = 0;
        }
        
        private bool DrawConditionalAction(SerializedProperty conditionalActionsProp, int index)
        {
            var delete = false;
            var conditionalAction = conditionalActionsProp.GetArrayElementAtIndex(index);
            
            using (new EditorGUILayout.VerticalScope("helpbox"))
            {
                EditorGUI.indentLevel++;
                var isOpenProp = conditionalAction.FindPropertyRelative("_isOpen");
                var actionDescriptionProp = conditionalAction.FindPropertyRelative("_actionDescription");
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(70)))
                    {
                        isOpenProp.boolValue = EditorGUILayout.Foldout(isOpenProp.boolValue, $"{index}", true);
                    }
                    EditorGUILayout.PropertyField(actionDescriptionProp, GUIContent.none);
                    if (OJEditorUtility.Button("+", GUI.backgroundColor, 30, EditorStyles.miniButtonLeft))
                    {
                        AddAnotherConditionalAction(conditionalActionsProp);
                    }
                    delete = OJEditorUtility.Button("x", GUI.backgroundColor, 30, EditorStyles.miniButtonRight);
                    
                }
                EditorGUI.indentLevel--;

                if(isOpenProp.boolValue)
                    DrawConditionGroups(conditionalAction);
                
                EditorGUILayout.Space();
            }
            return delete;
        }

        private void DrawConditionGroups(SerializedProperty conditionalAction)
        {
            var conditionGroupsProp = conditionalAction.FindPropertyRelative("_conditionGroups");
            EditorGUILayout.Space();
            for (var groupIndex = 0; groupIndex < conditionGroupsProp.arraySize; ++groupIndex)
            {
                var deleteConditionGroup = false;
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.Space(6, false);
                    using (new EditorGUILayout.VerticalScope("textArea"))
                    {
                        var conditionGroup = conditionGroupsProp.GetArrayElementAtIndex(groupIndex);
                        var conditions = conditionGroup.FindPropertyRelative("_conditions");
                        if (conditions.arraySize == 0)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.Space();
                                if (OJEditorUtility.Button("+", Color.green, 50, EditorStyles.miniButton))
                                {
                                    AddAnotherCondition(conditions, OJEditorUtility.GetLastRect($"conditionGroup_add_{groupIndex}"));
                                }
                                OJEditorUtility.SetLastRect($"conditionGroup_add_{groupIndex}");

                                deleteConditionGroup = OJEditorUtility.Button("-", Color.red, 50, EditorStyles.miniButton);
                                EditorGUILayout.Space();
                            }
                        }
                        else
                        {
                            for (var conditionIndex = 0; conditionIndex < conditions.arraySize; ++conditionIndex)
                            {
                                if (DrawCondition(conditions, conditionIndex, groupIndex))
                                {
                                    conditions.DeleteArrayElementAtIndex(conditionIndex);
                                    conditionIndex--;
                                }
                            }
                        }
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.Space(6, false);
                }
                EditorGUILayout.Space();

                if (deleteConditionGroup)
                {
                    conditionGroupsProp.DeleteArrayElementAtIndex(groupIndex);
                    groupIndex--;
                }
            }

            DrawAddConditionGroup(conditionGroupsProp);
            DrawActionWithinConditional(conditionalAction);
        }

        private bool DrawCondition(SerializedProperty conditions, int conditionIndex, int groupIndex)
        {
            var delete = false;
            var condition = conditions.GetArrayElementAtIndex(conditionIndex);
            var comparison = condition.FindPropertyRelative("_comparison");
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space(6, false);
                using (new EditorGUILayout.VerticalScope("helpbox"))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(condition.FindPropertyRelative("_observer"), GUIContent.none);
                        if (OJEditorUtility.Button("+", Color.green, 30, EditorStyles.miniButton))
                        {
                            AddAnotherCondition(conditions, OJEditorUtility.GetLastRect($"condition_and_{conditionIndex}_{groupIndex}"));
                        }
                        
                        OJEditorUtility.SetLastRect($"condition_and_{conditionIndex}_{groupIndex}");
                        delete = OJEditorUtility.Button("-", Color.red, 30, EditorStyles.miniButton);
                    }

                    EditorGUILayout.PropertyField(comparison, GUIContent.none, true);
                }
                EditorGUILayout.Space(6, false);
            }

            return delete;
        }

        private void AddAnotherCondition(SerializedProperty conditions, Rect? lastRect)
        {
            if (lastRect.HasValue)
            {
                var menu = ConditionsUtility.GetInitialConditionMenu(conditions);
                menu.DropDown(lastRect.Value);
            }
        }

        private void DrawAddConditionGroup(SerializedProperty conditionGroupsProp)
        {
            if (OJEditorUtility.CenteredButton("or", GUI.backgroundColor, 50, EditorStyles.miniButton))
            {
                conditionGroupsProp.arraySize++;
                var newGroup = conditionGroupsProp.GetArrayElementAtIndex(conditionGroupsProp.arraySize - 1);
                newGroup.FindPropertyRelative("_conditions").arraySize = 0;
            }
        }

        protected virtual void DrawActionWithinConditional(SerializedProperty conditionalAction)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(conditionalAction.FindPropertyRelative("_action"));
            EditorGUI.indentLevel--;
        }
    }
}