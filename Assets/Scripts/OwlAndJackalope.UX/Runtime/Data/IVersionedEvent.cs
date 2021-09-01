using System;

namespace OwlAndJackalope.UX.Runtime.Data
{
    public interface IVersionedEvent : IVersioned
    {
        event Action VersionChanged;
    }
}