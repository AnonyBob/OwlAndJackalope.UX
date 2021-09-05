using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.States.Serialized
{
    public interface ISerializedState
    {
        IState ConvertToState(IReference reference);
    }
}