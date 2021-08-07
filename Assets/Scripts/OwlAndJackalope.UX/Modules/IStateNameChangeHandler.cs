namespace OwlAndJackalope.UX.Modules
{
    public interface IStateNameChangeHandler
    {
        void HandleStateNameChange(string previousName, string newName);
    }
}