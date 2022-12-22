using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Binding;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        public static GenericMenu GetInitialConditionMenu(SerializedProperty conditionsList, Object referenceModule)
        {
            var genericMenu = new GenericMenu();
            foreach (var conditionTypeComparisons in _conditionTypes)
            {
                if (conditionTypeComparisons.Value.Count == 1)
                {
                    var conditionComparison = conditionTypeComparisons.Value[0];
                    genericMenu.AddItem(new GUIContent(conditionComparison.Display.FolderName ?? conditionComparison.Display.DisplayName), false, () =>
                    {
                        AddConditionMenuOption(conditionsList, conditionComparison, referenceModule);
                    });
                }
                else
                {
                    foreach (var conditionComparison in conditionTypeComparisons.Value)
                    {
                        genericMenu.AddItem(new GUIContent(conditionComparison.Display.FullName), false, () =>
                        {
                            AddConditionMenuOption(conditionsList, conditionComparison, referenceModule);
                        });
                    }
                }
            }

            return genericMenu;
        }

        private static void AddConditionMenuOption(SerializedProperty conditionsList, 
            ConditionComparisonListItem conditionComparison, Object referenceModule)
        {
            conditionsList.arraySize++;
            var conditionProp = conditionsList.GetArrayElementAtIndex(conditionsList.arraySize - 1);
            var observerProp = conditionProp.FindPropertyRelative("_observer");
            observerProp.FindPropertyRelative("_detailName").stringValue = string.Empty;
            if (referenceModule != null)
            {
                observerProp.FindPropertyRelative("_referenceModule").objectReferenceValue = referenceModule;    
            }

            var conditionComparisonInstance = Activator.CreateInstance(conditionComparison.ConditionComparisonType);
            conditionProp.FindPropertyRelative("_comparison").managedReferenceValue = conditionComparisonInstance;
            conditionProp.serializedObject.ApplyModifiedProperties();
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
                            var conditionValueType = GetConditionValueTypeInterface(type, conditionValueComparisonType);
                            if (conditionValueType != null)
                            {
                                AddDetailsForConditionValueType(type, conditionValueType);
                            }
                            else
                            {
                                AddDetailsForGenericCondition(type);
                            }
                        }
                    }
                    
                }
            }
        }

        private static Type GetConditionValueTypeInterface(Type type, Type conditionValueComparisonType)
        {
            return type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == conditionValueComparisonType);
        }

        private static void AddDetailsForConditionValueType(Type originalType, Type conditionValueType)
        {
            var valueType = conditionValueType.GenericTypeArguments[0];
            if (!_conditionTypes.TryGetValue(valueType, out var conditionTypeList))
            {
                conditionTypeList = new List<ConditionComparisonListItem>();
                _conditionTypes[valueType] = conditionTypeList;
            }
                            
            var displayAttribute = originalType.GetCustomAttribute<ConditionDisplayAttribute>();
            conditionTypeList.Add(new ConditionComparisonListItem()
            {
                Display = displayAttribute ?? new ConditionDisplayAttribute(originalType.Name),
                ConditionComparisonType = originalType,
                ValueType = valueType
            });
        }

        private static void AddDetailsForGenericCondition(Type type)
        {
            var instance = Activator.CreateInstance(type) as IConditionComparison;
            var valueType = instance!.GetConditionValueType();   
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

        private struct ConditionComparisonListItem
        {
            public ConditionDisplayAttribute Display { get; set; }
            public Type ConditionComparisonType { get; set; }
            public Type ValueType { get; set; }
        }
    }
}