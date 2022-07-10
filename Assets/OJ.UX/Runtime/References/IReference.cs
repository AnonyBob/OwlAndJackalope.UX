using System.Collections.Generic;
using OJ.UX.Runtime.Versions;

namespace OJ.UX.Runtime.References
{
    public interface IReference : IVersionable, IChangeable, IEnumerable<KeyValuePair<string, IDetail>>
    {
        IDetail GetDetail(string detailName);

        IDetail<TValue> GetDetail<TValue>(string detailName);
    }

    public interface IMutableReference : IReference
    {
        bool AddDetail(string detailName, IDetail detail, bool allowOverwrite = true);

        int AddDetails(IEnumerable<KeyValuePair<string, IDetail>> details, bool allowOverwrite = true);

        bool RemoveDetail(string detailName);
    }
}