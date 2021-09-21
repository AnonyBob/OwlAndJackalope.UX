using System;

namespace OwlAndJackalope.UX.Runtime.Data
{
    /// <summary>
    /// Container for a singular piece of data.
    /// </summary>
    public interface IDetail : IVersionedEvent, INameable
    {
        object GetObject();

        Type GetObjectType();
    }

    /// <summary>
    /// Container for a singular piece of data of the provided type.
    /// </summary>
    public interface IDetail<TValue> : IDetail
    {
        TValue GetValue();
    }

    /// <summary>
    /// Container for a singular piece of data of the provided type that can be updated.
    /// </summary>
    public interface IMutableDetail : IDetail
    {
        /// <summary>
        /// Sets the value of the data. If successful this will return true. If the values are the same then
        /// it will return false. Throws if the object type is not consistent with the detail.
        /// </summary>
        bool SetObject(object obj);
    } 
    
    /// <summary>
    /// Container for a singular piece of data of the provided type that can be updated by the provided type.
    /// </summary>
    public interface IMutableDetail<TValue> : IDetail<TValue>, IMutableDetail
    {
        /// <summary>
        /// Sets the value of the data. If successful this will return true. If the values are the same then
        /// it will return false.
        /// </summary>
        bool SetValue(TValue value);
    }
}