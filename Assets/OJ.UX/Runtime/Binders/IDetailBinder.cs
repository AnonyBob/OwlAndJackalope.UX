using OJ.UX.Runtime.Binding;

namespace OJ.UX.Runtime.Binders
{
    public interface IDetailBinder
    {
        /// <summary>
        /// If a detail on the changing module changes its name then we want to ensure that the
        /// serialization on us also changes. If the value does change then we should return true.
        /// </summary>
        bool RespondToNameChange(ReferenceModule changingModule, string originalName, string newName);
    }
}