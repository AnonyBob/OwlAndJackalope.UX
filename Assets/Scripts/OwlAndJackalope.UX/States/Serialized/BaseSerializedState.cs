using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Conditions.Serialized;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.States.Serialized
{
    [System.Serializable]
    public class BaseSerializedState : ISerializedState, IDetailNameChangeHandler
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private List<SerializedConditionAndGroup> _conditionGroups;

        public IState ConvertToState(IReference reference)
        {
            return new BaseRuntimeState(_name, reference, _conditionGroups
                .Select(x => x.ConvertToCondition())
                .Where(x => x != null));
        }

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            foreach (var condition in _conditionGroups)
            {
                condition.HandleDetailNameChange(previousName, newName, this);
            }
        }
    }
}