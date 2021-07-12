using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Conditions.Serialized;
using OwlAndJackalope.UX.Data;
using UnityEngine;

namespace OwlAndJackalope.UX.States.Serialized
{
    [System.Serializable]
    public class BaseSerializedState : ISerializedState
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
    }
}