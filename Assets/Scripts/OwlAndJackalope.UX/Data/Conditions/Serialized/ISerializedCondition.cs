namespace OwlAndJackalope.UX.Data.Conditions.Serialized
{
    public interface ISerializedCondition
    {
        ICondition ConvertToCondition();
    }
}