using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Modules;

namespace OwlAndJackalope.UX.States.Serialized
{
    public interface ISerializedState
    {
        IState ConvertToState(IReference reference);
    }
}