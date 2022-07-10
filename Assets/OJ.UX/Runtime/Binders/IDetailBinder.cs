using OJ.UX.Runtime.Binding;

namespace OJ.UX.Runtime.Binders
{
    public interface IDetailBinder
    {
        void RespondToNameChange(ReferenceModule changingModule, string originalName, string newName);
    }
}