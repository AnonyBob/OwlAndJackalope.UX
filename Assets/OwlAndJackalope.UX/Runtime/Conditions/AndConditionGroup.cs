using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.Conditions
{
    /// <summary>
    /// All internal conditions must be true for this to be true.
    /// </summary>
    public class AndConditionGroup : ICondition
    {
        private readonly List<ICondition> _conditions;
        
        public AndConditionGroup(IEnumerable<ICondition> conditions)
        {
            if (conditions == null)
            {
                _conditions = new List<ICondition>();
            }
            else
            {
                _conditions = conditions.ToList();    
            }
        }
        
        public IEnumerable<string> GetUsedDetails()
        {
            return _conditions.SelectMany(c => c.GetUsedDetails());
        }

        public bool IsMet(IReference reference)
        {
            if (_conditions.Count == 0)
            {
                return false;
            }
            
            foreach (var condition in _conditions)
            {
                if (!condition.IsMet(reference))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsMet(IReference reference, IDetail argument)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.IsMet(reference, argument))
                {
                    return false;
                }
            }

            return true;
        }
    }
}