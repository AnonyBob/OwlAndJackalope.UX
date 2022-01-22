using System.Linq;

namespace OwlAndJackalope.UX.Runtime.Data.Extensions
{
    public static class ReferenceExtensions
    {
        public static TValue GetValue<TValue>(this IReference reference, string name)
        {
            var detail = reference.GetDetail<TValue>(name);
            if (detail != null)
            {
                return detail.GetValue();
            }

            return default;
        }

        public static void SetValue<TValue>(this IReference reference, string name, TValue value)
        {
            var detail = reference.GetMutable<TValue>(name);
            if (detail != null)
            {
                detail.SetValue(value);
            }
        }
        
        public static IMutableDetail<TValue> GetMutable<TValue>(this IReference reference, string name)
        {
            return reference.GetDetail(name) as IMutableDetail<TValue>;
        }

        public static ICollectionDetail<TValue> GetCollection<TValue>(this IReference reference, string name)
        {
            return reference.GetDetail(name) as ICollectionDetail<TValue>;
        }
        
        public static IMutableCollectionDetail<TValue> GetMutableCollection<TValue>(this IReference reference, string name)
        {
            return reference.GetDetail(name) as IMutableCollectionDetail<TValue>;
        }

        public static IMapDetail<TKey, TValue> GetMap<TKey, TValue>(this IReference reference, string name)
        {
            return reference.GetDetail(name) as IMapDetail<TKey, TValue>;
        }
        
        public static IMutableMapDetail<TKey, TValue> GetMutableMap<TKey, TValue>(this IReference reference, string name)
        {
            return reference.GetDetail(name) as IMutableMapDetail<TKey, TValue>;
        }

        public static string ShortPrint(this IReference reference)
        {
            if (reference == null)
            {
                return null;
            }

            return $"{reference.GetType().Name} -- {reference.Count()}";
        }
    }
}