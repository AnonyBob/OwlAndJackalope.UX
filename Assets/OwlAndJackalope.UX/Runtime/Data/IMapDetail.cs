using System;
using System.Collections.Generic;

namespace OwlAndJackalope.UX.Runtime.Data
{
    /// <summary>
    /// Maintains a mapped collection of items.
    /// </summary>
    public interface IMapDetail : IDetail
    {
        (Type KeyType, Type ValueType) GetItemType();
    }
    
    /// <summary>
    /// A detail that acts as a simple dictionary of one item type to another. The version is updated whenever
    /// items are added or removed.
    /// </summary>
    public interface IMapDetail<TKey, TValue> : IDetail<Dictionary<TKey, TValue>>, IDictionary<TKey, TValue>, IMapDetail
    {
    }

    /// <summary>
    /// A detail that acts as a simple dictionary of one item type to another. The version is updated whenever
    /// items are added or removed. The mutable version can be set to directly to override the original value.
    /// This will also increase the version
    /// </summary>
    public interface IMutableMapDetail<TKey, TValue> : IMapDetail<TKey, TValue>,
        IMutableDetail<Dictionary<TKey, TValue>>
    {
        
    }
}