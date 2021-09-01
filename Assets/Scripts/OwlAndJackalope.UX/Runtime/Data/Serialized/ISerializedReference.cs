using System.Collections.Generic;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// A container for SerializedDetails that can when required be converted into a IReference.
    /// </summary>
    public interface ISerializedReference : IEnumerable<ISerializedDetail>
    {
        IReference ConvertToReference();
    }
}