using System;
using OJ.UX.Runtime.Versions;

namespace OJ.UX.Runtime.References
{
    public interface IDetail : IVersionable, IChangeable
    {
        object Value { get; }
        
        Type ValueType { get; }
    }

    public interface IDetail<TValue> : IDetail
    {
        new TValue Value { get; }
    }

    public interface IMutableDetail<TValue> : IDetail<TValue>
    {
        new TValue Value { get; set; }
    }
}