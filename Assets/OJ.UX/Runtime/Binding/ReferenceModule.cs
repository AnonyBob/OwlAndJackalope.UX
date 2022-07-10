using System;
using System.Collections.Generic;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.References.Serialized;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    [DefaultExecutionOrder(-100)]
    public sealed class ReferenceModule : MonoBehaviour
    {
        public IReference Reference
        {
            get
            {
                if (_runtimeReference == null)
                    InitializeRuntime();
                
                return _runtimeReference;
            }
            set
            {
                if (_runtimeReference == null)
                    InitializeRuntime();
                
                if(value != null)
                    AddDetails(value);
            }
        }

        [SerializeField]
        private SerializedReference _serializedReference;
        private IMutableReference _runtimeReference;

        private void OnEnable()
        {
            if(_runtimeReference == null)
                InitializeRuntime();
        }

        private void InitializeRuntime()
        {
            _runtimeReference = _serializedReference.CreateReference();
            var provider = GetComponent<DetailsProvider>();
            if (provider != null)
            {
                AddDetails(provider.ProvideDetails());
            }
        }

        private void AddDetails(IEnumerable<KeyValuePair<string, IDetail>> details)
        {
#if UNITY_EDITOR
            foreach (var detail in details)
            {
                _serializedReference.LinkDetail(detail.Key, detail.Value);
                _runtimeReference.AddDetail(detail.Key, detail.Value);
            }
#else
            _runtimeReference.AddDetails(details);
#endif
        }
    }
}