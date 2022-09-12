using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.References.Serialized;
using OJ.UX.Runtime.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace OJ.UX.Editor.References.Serialized
{
    public static class SerializedEditorReferenceUtility
    {
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
                if(detailsProp.arraySize > index)
                    EditorGUI.PropertyField(rect, detailsProp.GetArrayElementAtIndex(index), true);
            };

            list.elementHeightCallback = index =>
            {
                if(detailsProp.arraySize > index)
                    return EditorGUI.GetPropertyHeight(detailsProp.GetArrayElementAtIndex(index));
                return 0;
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

            foreach (var menuItem in SerializedReferenceUtility.GetMenuItems())
            {
                menu.AddItem(menuItem.ListDisplay, false, () =>
                {
                    listProp.arraySize++;
                    var itemProp = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
                    itemProp.managedReferenceValue = CreateDetailFromType(menuItem.DisplayName, menuItem.SerializedDetailType);

                    listProp.serializedObject.ApplyModifiedProperties();
                });
            }
            
            return menu;
        }

        private static ISerializedDetail CreateDetailFromType(string displayName, Type menuItemDetailType)
        {
            var detail = (AbstractSerializedDetail)Activator.CreateInstance(menuItemDetailType);
            detail.Name = $"{displayName} {Guid.NewGuid().ToString().Substring(0, 5)}";
            return detail;
        }
    }
}