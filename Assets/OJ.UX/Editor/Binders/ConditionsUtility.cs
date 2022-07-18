using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.Binders.Conditions;
using UnityEditor;
using UnityEngine;

namespace OJ.UX.Editor.Binders
{
    public static class ConditionsUtility
    {
        private static readonly Dictionary<Type, List<ConditionComparisonListItem>> _conditionTypes =
            new Dictionary<Type, List<ConditionComparisonListItem>>();

        static ConditionsUtility()
        {
            CreateConditionTypesDictionary();
        }

        public static GenericMenu GetInitialConditionMenu(SerializedProperty conditionsList)
        {
            var genericMenu = new GenericMenu();
            foreach (var conditionTypeComparisons in _conditionTypes)
            {
                var firstConditionComparison = conditionTypeComparisons.Value[0];
                genericMenu.AddItem(new GUIContent(firstConditionComparison.Display.FolderName ?? firstConditionComparison.ValueType.Name), false, () =>
                {
                    conditionsList.arraySize++;
                    var conditionProp = conditionsList.GetArrayElementAtIndex(conditionsList.arraySize - 1);
                    var observerProp = conditionProp.FindPropertyRelative("_observer");
                    observerProp.FindPropertyRelative("_detailName").stringValue = string.Empty;
                    
                    var conditionComparisonInstance = Activator.CreateInstance(firstConditionComparison.ConditionComparisonType);
                    conditionProp.FindPropertyRelative("_comparison").managedReferenceValue = conditionComparisonInstance;
                    conditionProp.serializedObject.ApplyModifiedProperties();
                });
            }

            return genericMenu;
        }
        
        private static void CreateConditionTypesDictionary()
        {
            _conditionTypes.Clear();
            var conditionComparisonType = typeof(IConditionComparison);
            var conditionValueComparisonType = typeof(IConditionValueComparison<>);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    //No interfaces or abstract types only concrete classes
                    if (!type.IsInterface && !type.IsAbstract)
                    {
                        //If it is a condition.
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Contains(conditionComparisonType))
                        {
                            var conditionValueType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType 
                                && i.GetGenericTypeDefinition() == conditionValueComparisonType);
                            if(conditionValueType == null) 
                                continue;

                            var valueType = conditionValueType.GenericTypeArguments[0];
                            if (!_conditionTypes.TryGetValue(valueType, out var conditionTypeList))
                            {
                                conditionTypeList = new List<ConditionComparisonListItem>();
                                _conditionTypes[valueType] = conditionTypeList;
                            }
                            
                            var displayAttribute = type.GetCustomAttribute<ConditionDisplayAttribute>();
                            conditionTypeList.Add(new ConditionComparisonListItem()
                            {
                                Display = displayAttribute ?? new ConditionDisplayAttribute(type.Name),
                                ConditionComparisonType = type,
                                ValueType = valueType
                            });
                        }
                    }
                    
                }
            }
        }

        private struct ConditionComparisonListItem
        {
            public ConditionDisplayAttribute Display { get; set; }
            public Type ConditionComparisonType { get; set; }
            public Type ValueType { get; set; }
        }
    }
}