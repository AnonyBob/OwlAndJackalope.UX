using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.States.Serialized
{
    public interface ISerializedState
    {
        IState ConvertToState(IReference reference);
    }
}