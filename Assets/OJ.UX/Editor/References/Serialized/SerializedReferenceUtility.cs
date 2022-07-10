﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.References.Serialized;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    public static class SerializedReferenceUtility
    {
        private static readonly List<DetailListMenuItem> _menuItems;
        
        static SerializedReferenceUtility()
        {
            _menuItems = CreateDetailSelectionMenu();
        }

        public static ReorderableList CreateDetailList(GUIContent label, SerializedProperty detailsProp, GenericMenu menu)
        {
            var list = new ReorderableList(detailsProp.serializedObject, detailsProp, 
                true, true, true, true);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, label);
            };

            list.drawElementCallback = (rect, index, active, focused) =>
            {
                EditorGUI.PropertyField(rect, detailsProp.GetArrayElementAtIndex(index), true);
            };

            list.elementHeightCallback = index =>
            {
                return EditorGUI.GetPropertyHeight(detailsProp.GetArrayElementAtIndex(index));
            };

            list.onAddDropdownCallback = (rect, list) =>
            {
                menu.DropDown(rect);
            };

            return list;
        }
        
        public static GenericMenu CreateSelectionMenu(SerializedProperty listProp)
        {
            var menu = new GenericMenu();

            foreach (var menuItem in _menuItems)
            {
                menu.AddItem(menuItem.DisplayContent, false, () =>
                {
                    listProp.arraySize++;
                    var itemProp = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
                    itemProp.managedReferenceValue = CreateDetailFromType(menuItem.DisplayContent, menuItem.DetailType);

                    listProp.serializedObject.ApplyModifiedProperties();
                });
            }
            
            return menu;
        }

        private static ISerializedDetail CreateDetailFromType(GUIContent displayName, Type menuItemDetailType)
        {
            var detail = (AbstractSerializedDetail)Activator.CreateInstance(menuItemDetailType);
            detail.Name = $"{displayName.text} {Guid.NewGuid().ToString().Substring(0, 5)}";
            return detail;
        }

        private static List<DetailListMenuItem> CreateDetailSelectionMenu()
        {
            var items = new List<DetailListMenuItem>();
            var abstractSerializedDetailType = typeof(AbstractSerializedDetail);
            var serializedValueInterfaceType = typeof(ISerializedValueDetail<>);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsInterface && !type.IsAbstract)
                    {
                        if (type.IsSubclassOf(abstractSerializedDetailType))
                        {
                            var valueType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType 
                                && i.GetGenericTypeDefinition() == serializedValueInterfaceType);
                            
                            if (valueType != null)
                            {
                                var genericType = valueType.GetGenericArguments()[0];
                                var displayContent = genericType.Name;

                                var displayHelper = type.GetCustomAttribute<SerializedDetailDisplayAttribute>();
                                if (displayHelper != null)
                                {
                                    displayContent = displayHelper.DisplayName;
                                }
                                
                                items.Add(new DetailListMenuItem()
                                {
                                    DetailType = type,
                                    DisplayContent = new GUIContent(displayContent)
                                });
                            }
                            
                        }
                    }
                }
            }

            return items;
        }

        private struct DetailListMenuItem
        {
            public GUIContent DisplayContent { get; set; }
            public Type DetailType { get; set; }
        }
    }
}