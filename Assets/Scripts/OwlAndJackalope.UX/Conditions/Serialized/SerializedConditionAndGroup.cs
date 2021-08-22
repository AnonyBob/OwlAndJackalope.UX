using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Conditions.Serialized
{
    [System.Serializable]
    public class SerializedConditionAndGroup : IDetailNameChangeHandler, ISerializedCondition
    {
        [SerializeField]
        private List<BaseSerializedCondition> _conditions;

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            foreach (var condition in _conditions)
            {
                condition.HandleDetailNameChange(previousName, newName, this);
            }
        }

        public ICondition ConvertToCondition()
        {
            return new AndConditionGroup(_conditions
                .Select(c => c.ConvertToCondition())
                .Where(c => c != null));
        }
    }
}