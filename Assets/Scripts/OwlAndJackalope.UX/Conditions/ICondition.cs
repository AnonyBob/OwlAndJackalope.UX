using System.Collections.Generic;
using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.Conditions
{
    public interface ICondition
    {
        IEnumerable<string> GetUsedDetails();
        
        bool IsMet(IReference reference);
        
        bool IsMet(IReference reference, IDetail argument);
    }
}