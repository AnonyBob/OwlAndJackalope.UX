namespace OwlAndJackalope.UX.Data.Serialized
{
    /// <summary>
    /// A container that can be initialized into a detail.
    /// </summary>
    public interface ISerializedDetail
    {
        IDetail ConvertToDetail();
    }
}