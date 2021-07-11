using System;

namespace OwlAndJackalope.UX.Data.State
{
    public interface IState
    {
        bool IsActive { get; }
        
        event Action OnStateActiveChanged;
    }
}