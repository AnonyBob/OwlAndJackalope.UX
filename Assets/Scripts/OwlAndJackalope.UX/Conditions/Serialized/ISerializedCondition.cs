using OwlAndJackalope.UX.Modules;

namespace OwlAndJackalope.UX.Conditions.Serialized
{
    public interface ISerializedCondition
    {
        ICondition ConvertToCondition();
    }
}