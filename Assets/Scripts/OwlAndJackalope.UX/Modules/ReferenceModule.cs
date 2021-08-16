using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    public sealed class ReferenceModule : MonoBehaviour, IDetailNameChangeHandler
    {
        public IReference Reference
        {
            get => _runtimeReference;
            set
            {
                if (_runtimeReference == null)
                {
                    Initialize();
                }

                if (value != null)
                {
                    _runtimeReference.AddDetails(value);    
                }
                
            }
        }

        public BaseSerializedReference SerializedReference => _reference;
        
        [SerializeField]
        private BaseSerializedReference _reference;
        private IMutableReference _runtimeReference;

        private void Awake()
        {
            Initialize();
            
            var provider = GetComponent<ReferenceProvider>();
            if (provider != null)
            {
                Reference = provider.ProvideReference();
            }

            foreach (var module in GetComponents<IReferenceDependentModule>())
            {
                module.Initialize(Reference);
            }
        }

        private void Initialize()
        {
            _runtimeReference = new BaseReference(_reference.ConvertToReference());
        }
        
        public void HandleDetailNameChange(string previousName, string newName)
        {
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