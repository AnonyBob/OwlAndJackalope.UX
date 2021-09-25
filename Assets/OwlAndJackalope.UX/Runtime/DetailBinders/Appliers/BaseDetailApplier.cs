using OwlAndJackalope.UX.Runtime.Observers;

namespace OwlAndJackalope.UX.Runtime.DetailBinders.Appliers
{
    /// <summary>
    /// Inherit from this to flag certain binders as Appliers. Appliers are able to apply changes to the
    /// Details that they bind to.
    /// </summary>
    public abstract class BaseDetailApplier : BaseDetailBinder
    {
        protected bool IsReadyToSet<T>(MutableDetailObserver<T> observer)
        {
            if (!observer.IsSet)
            {
                observer.Initialize(_referenceModule.Reference);
                if (!observer.IsSet)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

