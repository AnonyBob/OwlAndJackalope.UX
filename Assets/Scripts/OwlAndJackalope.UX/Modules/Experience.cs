using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Modules.Initializers;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    public class Experience : MonoBehaviour, IDetailNameChangeHandler, IStateNameChangeHandler, IReferenceProvider
    {
        public IReference Reference
        {
            get => _referenceModule.Reference;
            set => _referenceModule.Reference = value;
        }

        public StateModule States => _stateModule;

        [SerializeField] private ReferenceModule _referenceModule;
        [SerializeField] private ActionModule _actionModule;
        [SerializeField] private StateModule _stateModule;

        private void Awake()
        {
            _referenceModule.Initialize(GetComponent<ExperienceReferenceProvider>());
            _stateModule.Initialize(_referenceModule.Reference);
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
    }
}