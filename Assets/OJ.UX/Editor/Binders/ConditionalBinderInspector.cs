using OJ.UX.Runtime.Binders;
using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    [CustomEditor(typeof(ConditionalBinder<>), true)]
    public class ConditionalBinderInspector : UnityEditor.Editor
    {
        private ReorderableList _conditionalActionList;
        
        public override void OnInspectorGUI()
        {
            if (_conditionalActionList == null)
            {
                var conditionalActionsProp = serializedObject.FindProperty("_conditionalActions");
                _conditionalActionList = new ReorderableList(serializedObject, 
                    conditionalActionsProp, true, true, true, true);
            
                _conditionalActionList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Conditional Actions");
                _conditionalActionList.drawElementCallback = (rect, index, active, focused) =>
                {
                    if (conditionalActionsProp.arraySize > index)
                        EditorGUI.PropertyField(rect, conditionalActionsProp.GetArrayElementAtIndex(index));
                };
                
                _conditionalActionList.elementHeightCallback = (index) =>
                {
                    if(conditionalActionsProp.arraySize > index)
                        return EditorGUI.GetPropertyHeight(conditionalActionsProp.GetArrayElementAtIndex(index));
                    return 0;
                };

                _conditionalActionList.onAddCallback = list =>
                {
                    AddAnotherConditionalAction(list.serializedProperty);
                };
            }
            
            _conditionalActionList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void AddAnotherConditionalAction(SerializedProperty conditionalActionsProp)
        {
            conditionalActionsProp.InsertArrayElementAtIndex(conditionalActionsProp.arraySize);
            var conditionalAction = conditionalActionsProp.GetArrayElementAtIndex(conditionalActionsProp.arraySize - 1);
            conditionalAction.FindPropertyRelative("_actionDescription").stringValue = string.Empty;
            conditionalAction.FindPropertyRelative("_conditionGroups").arraySize = 0;
        }
    }
}