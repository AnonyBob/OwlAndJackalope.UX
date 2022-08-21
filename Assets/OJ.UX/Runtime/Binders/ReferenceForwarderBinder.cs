using System;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class ReferenceForwarderBinder : AbstractDetailBinder
    {
        [SerializeField]
        private Observer<IReference> _reference;

        [SerializeField]
        private ReferenceModule _targetModule;

        private void Start()
        {
            if (_targetModule == null)
            {
                _targetModule = GetComponent<ReferenceModule>();
            }
            
            _reference.Initialize(HandleUpdate);
        }

        private void HandleUpdate()
        {
            if (_targetModule != null)
            {
                _targetModule.Reference = _reference.Value;
            }
        }

        private void OnDestroy()
        {
            _reference.Destroy();
        }
    }
}