using System;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Modules.Initializers;
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
                _reference = value;
                EstablishStateConnections();
            }
        }
        
        [SerializeField] private ReferenceModule _referenceModule;
        [SerializeField] private ActionModule _actionModule;
        [SerializeField] private StateModule _stateModule;

        private IReference _reference;
        
        private void Start()
        {
            if (Reference == null)
            {
                Reference = _referenceModule.GetReference();
            }
        }

        private void EstablishStateConnections()
        {
            //TODO: Link the states to parameter changes.
        }

        public void HandleDetailNameChange(string previousName, string newName)
        {
            if (Application.IsPlaying(this))
            {
                return;
            }
            
            Debug.Log($"Doing name change: {previousName} --> {newName}");
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