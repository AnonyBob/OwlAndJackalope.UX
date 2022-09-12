using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OJ.UX.Runtime.References.Serialized;
using UnityEngine;

namespace OJ.UX.Runtime.Utility
{
    public struct DetailListMenuItem
    {
        public GUIContent ListDisplay { get; set; }
        public string DisplayName { get; set; }
        public Type SerializedDetailType { get; set; }
        public Type ValueType { get; set; }
    }
    
    public static class SerializedReferenceUtility
    {
        private static readonly List<DetailListMenuItem> _menuItems = new List<DetailListMenuItem>();
        private static readonly Dictionary<Type, DetailListMenuItem> _typeToSerializedType = new Dictionary<Type, DetailListMenuItem>();

        static SerializedReferenceUtility()
        {
            CreateDetailSelectionMenu();
        }

        public static Type GetSerializedDetailType(Type valueType)
        {
            if (_typeToSerializedType.TryGetValue(valueType, out var menuItem))
            {
                return menuItem.SerializedDetailType;
            }

            return null;
        }

        public static IEnumerable<DetailListMenuItem> GetMenuItems()
        {
            return _menuItems;
        }
        
        private static void CreateDetailSelectionMenu()
        {
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
                                    displayContent = displayHelper.FullName;
                                }

                                var menuItem = new DetailListMenuItem()
                                {
                                    SerializedDetailType = type,
                                    ListDisplay = new GUIContent(displayContent),
                                    DisplayName = displayHelper?.DisplayName ?? displayContent,
                                    ValueType = genericType
                                };
                                
                                _menuItems.Add(menuItem);
                                _typeToSerializedType[genericType] = menuItem;
                            }
                            
                        }
                    }
                }
            }
        }
    }
}