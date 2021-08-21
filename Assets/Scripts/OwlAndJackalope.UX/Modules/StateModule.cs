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
    [System.Serializable, RequireComponent(typeof(ReferenceModule))]
    public sealed class StateModule : MonoBehaviour, IReferenceDependentModule, IStateNameChangeHandler, IDetailNameChangeHandler
    {
        [SerializeField]
        private List<BaseSerializedState> _states;
        private readonly Dictionary<string, IState> _runtimeStates = new Dictionary<string, IState>();
        
        public void Initialize(IReference reference)
        {
            _runtimeStates.Clear();
            foreach (var state in _states)
            {
                var runtimeState = state.ConvertToState(reference);
                if (_runtimeStates.ContainsKey(runtimeState.Name))
                {
                    Debug.LogWarning($"State Name: {runtimeState.Name} is already in use.");
                }
                _runtimeStates[runtimeState.Name] = runtimeState;
            }
        }

        public IState GetState(string name)
        {
            if (name != null && _runtimeStates.TryGetValue(name, out var state))
            {
                return state;
            }

            return null;
        }

        public void HandleStateNameChange(string previousName, string newName)
        {
            foreach (var handler in GetComponentsInChildren<IStateNameChangeHandler>())
            {
                if (!ReferenceEquals(handler, this))
                {
                    handler.HandleStateNameChange(previousName, newName);
                }
            }
        }

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            if (GetComponentInParent<ReferenceModule>() == root)
            {
                foreach (var state in _states)
                {
                    state.HandleDetailNameChange(previousName, newName, this);
                }
            }
            
                        
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}