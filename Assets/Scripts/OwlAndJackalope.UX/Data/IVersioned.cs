namespace OwlAndJackalope.UX.Data
{
    /// <summary>
    /// A simple wrapper that allows us to check the version of an object.
    /// </summary>
    public interface IVersioned
    {
        /// <summary>
        /// Data with the same value should maintain the same version number.
        /// </summary>
        long Version { get; }
    }
}