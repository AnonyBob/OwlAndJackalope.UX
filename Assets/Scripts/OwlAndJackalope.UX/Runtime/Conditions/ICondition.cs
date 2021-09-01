using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.Conditions
{
    public interface ICondition
    {
        IEnumerable<string> GetUsedDetails();
        
        bool IsMet(IReference reference);
        
        bool IsMet(IReference reference, IDetail argument);
    }
}