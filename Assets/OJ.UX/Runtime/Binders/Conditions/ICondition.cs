using System;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public interface ICondition : IDisposable
    {
        void Initialize(IConditionChangedHandler handler);
        
        bool IsConditionMet();
    }
}