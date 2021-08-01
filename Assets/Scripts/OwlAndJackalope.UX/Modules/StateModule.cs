using System.Collections.Generic;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.States;
using OwlAndJackalope.UX.States.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    /// <summary>
    /// Maintains the relationship between the reference's details and the various defined states.
    /// The states can be used like a state machine to setup different UX experiences. State relationships
    /// cannot be modified by external providers.
    /// </summary>
    [System.Serializable]
    public class StateModule
    {
        [SerializeField]
        private List<BaseSerializedState> _states;

        public void Initialize(IReference reference)
        {
            
        }
    }
}