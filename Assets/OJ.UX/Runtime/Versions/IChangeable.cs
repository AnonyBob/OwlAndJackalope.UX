using System;

namespace OJ.UX.Runtime.Versions
{
    public interface IChangeable
    {
        event Action OnChanged;
    }

    public interface IChangeable<TValue>
    {
        event Action<TValue> OnChanged;
    }
}