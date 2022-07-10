using System;

namespace OJ.UX.Runtime.References.Serialized
{
    public class SerializedDetailDisplayAttribute : Attribute
    {
        public readonly string DisplayName;

        public SerializedDetailDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}