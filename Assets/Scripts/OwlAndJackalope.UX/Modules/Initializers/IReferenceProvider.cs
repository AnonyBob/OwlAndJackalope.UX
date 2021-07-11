using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.Modules.Initializers
{
    /// <summary>
    /// A mechanism for retrieving a specific reference.
    /// </summary>
    public interface IReferenceProvider
    {
        IReference GetReference();
    }
}