using System;

namespace OwlAndJackalope.UX.States
{
    public interface IState
    {
        bool IsActive { get; }

        string Name { get; }

        event Action OnStateActiveChanged;
    }
}