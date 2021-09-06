using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.States.Serialized
{
    public interface ISerializedState
    {
        string Name { get; }
        
        IState ConvertToState(IReference reference);
    }
}