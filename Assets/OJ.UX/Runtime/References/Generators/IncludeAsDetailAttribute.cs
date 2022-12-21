using System;

namespace OJ.UX.Runtime.References.Generators
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IncludeAsDetailAttribute : Attribute
    {
        public readonly bool MakeMutable;

        public IncludeAsDetailAttribute(bool makeMutable = false)
        {
            MakeMutable = makeMutable;
        }
    }
}