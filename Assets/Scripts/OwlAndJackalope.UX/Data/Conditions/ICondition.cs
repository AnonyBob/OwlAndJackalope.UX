using System.Collections.Generic;

namespace OwlAndJackalope.UX.Data.Conditions
{
    public interface ICondition
    {
        IEnumerable<string> GetUsedDetails();
        
        bool IsMet(IReference reference);
        
        bool IsMet(IReference reference, IDetail argument);
    }
}