namespace OwlAndJackalope.UX.Runtime.Modules
{
    public interface IStateNameChangeHandler
    {
        void HandleStateNameChange(string previousName, string newName, IStateNameChangeHandler root);
    }
}