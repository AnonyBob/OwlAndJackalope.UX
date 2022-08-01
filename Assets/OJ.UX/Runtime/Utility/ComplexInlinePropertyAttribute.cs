using UnityEngine;

namespace OJ.UX.Runtime.Utility
{
    public class ComplexInlinePropertyAttribute : PropertyAttribute
    {
        public readonly string[] PropertyFields;

        public ComplexInlinePropertyAttribute(params string[] propertyFields)
        {
            PropertyFields = propertyFields;
        }
    }
}