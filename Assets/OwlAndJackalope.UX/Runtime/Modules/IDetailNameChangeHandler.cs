namespace OwlAndJackalope.UX.Runtime.Modules
{
    public interface IDetailNameChangeHandler
    {
        void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root);
    }
}