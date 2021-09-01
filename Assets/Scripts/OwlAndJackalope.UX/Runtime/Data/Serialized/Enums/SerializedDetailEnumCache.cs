using System;
using System.Collections.Generic;
using System.Linq;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized.Enums
{
    public static class SerializedDetailEnumCache
    {
        private static readonly Dictionary<int, IEnumDetailCreator> EnumCreators = new Dictionary<int, IEnumDetailCreator>();
        private static string[] _cachedEnumTypeNames;
        
        public static string[] EnumTypeNames
        {
            get
            {
                if (_cachedEnumTypeNames == null || _cachedEnumTypeNames.Length != EnumCreators.Count)
                {
                    _cachedEnumTypeNames = EnumCreators.Values.Select(x => x.EnumName).ToArray();
                }

                return _cachedEnumTypeNames;
            }
        }

        public static IEnumDetailCreator GetCreator(int enumId)
        {
            EnumCreators.TryGetValue(enumId, out var creator);
            return creator;
        }

        public static (int EnumId, IEnumDetailCreator Creator) GetCreator(string typeName)
        {
            var kvp = EnumCreators.FirstOrDefault(x => x.Value.EnumName == typeName);
            return (kvp.Key, kvp.Value);
        }

        public static void AddEnumType(int enumId, IEnumDetailCreator creator)
        {
            EnumCreators[enumId] = creator;
        }
    }
}