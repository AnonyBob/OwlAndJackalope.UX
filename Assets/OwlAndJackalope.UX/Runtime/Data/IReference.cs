using System;
using System.Collections.Generic;

namespace OwlAndJackalope.UX.Runtime.Data
{
    /// <summary>
    /// A container for a collection of details.
    /// </summary>
    public interface IReference : IVersionedEvent, IEnumerable<IDetail>, IEquatable<IReference>
    {
        IDetail GetDetail(string name);

        IDetail<TValue> GetDetail<TValue>(string name);
    }

    /// <summary>
    /// A container for a collection of details that can be altered over time.
    /// </summary>
    public interface IMutableReference : IReference
    {
        bool AddDetail(IDetail detail, bool overwrite = true);

        int AddDetails(IEnumerable<IDetail> details, bool overwrite = true);

        bool RemoveDetail(string detailName);
    }
}