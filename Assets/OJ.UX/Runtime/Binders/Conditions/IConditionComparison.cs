using System;
using OJ.UX.Runtime.Binding;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public interface IConditionComparison
    {
        Type GetConditionValueType();
        
        bool CheckCondition(Observer observer);
    }
}