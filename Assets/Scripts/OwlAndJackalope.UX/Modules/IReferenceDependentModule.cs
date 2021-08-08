using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.Modules
{
    public interface IReferenceDependentModule
    {
        void Initialize(IReference reference);
    }
}