namespace OwlAndJackalope.UX.Data.Extensions
{
    public static class ReferenceExtensions
    {
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
    }
}