using System;

namespace OwlAndJackalope.UX.Runtime.States
{
    public interface IState
    {
        bool IsActive { get; }

        string Name { get; }

        event Action OnStateActiveChanged;
    }
}