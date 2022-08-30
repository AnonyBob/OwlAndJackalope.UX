using System;
using System.Collections.Generic;
using System.Linq;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.References.Serialized;
using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    [DefaultExecutionOrder(-100)]
    public sealed class ReferenceModule : MonoBehaviour, IInitializableGameObject<IReference>
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

        public GameObject GameObject => this.gameObject;

        [SerializeField]
        private SerializedReference _serializedReference;
        private IMutableReference _runtimeReference;

        public void Initialize(IReference value)
        {
            Reference = value;
        }

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

#if UNITY_EDITOR
        public bool Editor_CheckName(string name)
        {
            if (_serializedReference?.Details != null)
            {
                return _serializedReference.Details.All(d => d.GetName() != name);
            }

            return true;
        }
#endif
        
    }
}