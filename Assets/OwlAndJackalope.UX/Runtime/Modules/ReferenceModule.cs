using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Modules
{
    public sealed class ReferenceModule : MonoBehaviour, IDetailNameChangeHandler
    {
        public IReference Reference
        {
            get => _runtimeReference;
            set => AddDetails(value);
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
                AddDetails(provider.ProvideReference());
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

        public void AddDetails(IEnumerable<IDetail> details)
        {
            if (_runtimeReference == null)
            {
                Initialize();
            }

            _runtimeReference.AddDetails(details);   
#if UNITY_EDITOR
            _reference.UpdateSerializedDetails(_runtimeReference);
#endif
        }
        
        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            if (root == null)
            {
                foreach (var handler in GetComponentsInChildren<IDetailNameChangeHandler>())
                {
                    if (!ReferenceEquals(handler, this))
                    {
                        handler.HandleDetailNameChange(previousName, newName, this);
                    }
                }
            }
        }
    }
}