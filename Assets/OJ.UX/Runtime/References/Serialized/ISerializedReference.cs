using System.Collections.Generic;

namespace OJ.UX.Runtime.References.Serialized
{
    public interface ISerializedReference
    {
        IMutableReference CreateReference();

        IEnumerable<ISerializedDetail> CopyDetails();

        void AddDetails(IEnumerable<ISerializedDetail> details);
    }
}