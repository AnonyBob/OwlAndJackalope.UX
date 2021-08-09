using System;
using System.Collections.Generic;
using System.Linq;

namespace OwlAndJackalope.UX.Data.Serialized.Editor.EnumExtensions
{
    public static class SerializedDetailEnumCache
    {
        private static readonly List<Type> _enumTypes = new List<Type>();
        private static string[] _cachedEnumTypeNames;
        private static string[] _cachedEnumTypeFullNames;
        public static string[] EnumTypeNames
        {
            get
            {
                if (_cachedEnumTypeNames == null || _cachedEnumTypeNames.Length != _enumTypes.Count)
                {
                    _cachedEnumTypeNames = _enumTypes.Select(x => x.Name).ToArray();
                }

                return _cachedEnumTypeNames;
            }
        }
        
        public static string[] EnumTypeFullNames
        {
            get
            {
                if (_cachedEnumTypeFullNames == null || _cachedEnumTypeFullNames.Length != _enumTypes.Count)
                {
                    _cachedEnumTypeFullNames = _enumTypes.Select(x => x.FullName).ToArray();
                }

                return _cachedEnumTypeFullNames;
            }
        }

        public static IEnumerable<Type> EnumTypes => _enumTypes;

        public static void AddEnumType(Type type)
        {
            if (type.IsEnum && !EnumTypes.Contains(type))
            {
                _enumTypes.Add(type);
                _enumTypes.Sort((type1, type2) => type1.Name.CompareTo(type2.Name));
            }
        }

        public static Type GetEnumType(string name)
        {
            return EnumTypes.FirstOrDefault(x => x.Name == name);
        }
    }
}