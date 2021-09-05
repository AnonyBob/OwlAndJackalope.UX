namespace OwlAndJackalope.UX.Runtime.Conditions.Serialized
{
    public interface ISerializedCondition
    {
        ICondition ConvertToCondition();
    }
}