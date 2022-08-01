using System;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public interface ICondition 
    {
        void Initialize(IConditionChangedHandler handler);

        void Destroy();
        
        bool IsConditionMet();
    }
}