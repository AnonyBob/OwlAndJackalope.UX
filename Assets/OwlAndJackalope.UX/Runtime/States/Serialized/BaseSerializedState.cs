using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Conditions.Serialized;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.States.Serialized
{
    [System.Serializable]
    public class BaseSerializedState : ISerializedState, IDetailNameChangeHandler
    {
        public string Name => _name;
        
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