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
        private List<BaseSerializedCondition> _conditions;

        public IState ConvertToState(IReference reference)
        {
            return new BaseRuntimeState(_name, reference, _conditions
                .Select(x => x.ConvertToCondition())
                .Where(x => x != null));
        }

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            foreach (var condition in _conditions)
            {
                condition.HandleDetailNameChange(previousName, newName, this);
            }
        }
    }
}