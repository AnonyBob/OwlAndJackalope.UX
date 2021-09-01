using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.Modules
{
    public interface IReferenceDependentModule
    {
        void Initialize(IReference reference);
    }
}