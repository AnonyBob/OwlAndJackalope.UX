namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// A container for a series of SerializedDetails. This can be when required converted into a
    /// standard IReference to be used by other systems.
    /// </summary>
    [System.Serializable]
    public class BaseSerializedReference : AbstractSerializedReference<BaseSerializedDetail, BaseSerializedCollectionDetail, BaseSerializedMapDetail>
    {
    }
}