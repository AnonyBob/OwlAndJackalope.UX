using System.Collections.Generic;

namespace OwlAndJackalope.UX.Data
{
    /// <summary>
    /// Manages a collection of objects. Collection details are updated when contents are added or removed.
    /// </summary>
    public interface ICollectionDetail<TValue> : IDetail<List<TValue>>, IList<TValue>
    {
    }

    /// <summary>
    /// Manages a collection of objects. Collection details are updated when contents are added or removed.
    /// Mutable collections can have their entire value set to a new value.
    /// </summary>
    public interface IMutableCollectionDetail<TValue> : ICollectionDetail<TValue>, IMutableDetail<List<TValue>>
    {
    }
}