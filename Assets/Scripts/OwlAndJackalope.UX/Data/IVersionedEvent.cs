using System;

namespace OwlAndJackalope.UX.Data
{
    public interface IVersionedEvent : IVersioned
    {
        event Action VersionChanged;
    }
}