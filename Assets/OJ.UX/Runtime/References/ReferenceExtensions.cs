using System.Collections.Generic;

namespace OJ.UX.Runtime.References
{
    public static class ReferenceExtensions
    {
        public static IMutableDetail<T> GetMutableDetail<T>(this IReference reference, string detailName)
        {
            return reference.GetDetail<T>(detailName) as IMutableDetail<T>;
        }

        public static ListDetail<T> GetListDetail<T>(this IReference reference, string detailName)
        {
            return reference.GetDetail<List<T>>(detailName) as ListDetail<T>;
        }
    }
}