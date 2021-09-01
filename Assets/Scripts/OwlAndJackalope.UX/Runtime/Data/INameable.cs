namespace OwlAndJackalope.UX.Runtime.Data
{
    /// <summary>
    /// Makes an identifiable piece of data.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        /// A unique name that can be used as a human readable element.
        /// </summary>
        string Name { get; }
    }
}