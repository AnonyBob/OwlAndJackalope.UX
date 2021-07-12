using System.Collections.Generic;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.States;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    public class Experience : MonoBehaviour, IDetailNameChangeHandler
    {
        public IReference Reference
        {
            get => _reference;
            set
            {
                if (_reference == null)
                {
                    _reference = new BaseReference(_referenceModule.GetReference());
                }
                
                _reference.AddDetails(value);
                CreateStatesIfNeeded();
            }
        }
        
        [SerializeField] private ReferenceModule _referenceModule;
        [SerializeField] private ActionModule _actionModule;
        [SerializeField] private StateModule _stateModule;

        private IMutableReference _reference;
        private Dictionary<string, IState> _states;
        
        private void Start()
        {
            if (_reference == null)
            {
                _reference = new BaseReference(_referenceModule.GetReference());
            }
        }

        private void CreateStatesIfNeeded()
        {
            if (_states == null)
            {
                //_states = _stateModule.GetStates(Reference);
            }
        }

        public void HandleDetailNameChange(string previousName, string newName)
        {
            if (Application.IsPlaying(this))
            {
                return;
            }
            
            foreach (var handler in GetComponentsInChildren<IDetailNameChangeHandler>())
            {
                if (!ReferenceEquals(handler, this))
                {
                    handler.HandleDetailNameChange(previousName, newName);
                }
            }
        }
    }
}