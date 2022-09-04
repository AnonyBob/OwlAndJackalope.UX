using System.Collections.Generic;
using System.Linq;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    [RequireComponent(typeof(ReferenceModule))]
    public class ReferenceInitializationObject : DetailsProvider, IInitializableGameObject<IReference>
    {
        private IReference _reference;
        public IReference Reference => _reference;

        public GameObject GameObject => gameObject;
        
        public void Initialize(IReference value)
        {
            _reference = value;
            var module = GetComponent<ReferenceModule>();
            if (module != null && _reference != null)
            {
                module.Reference = _reference;
            }
        }

        public override IEnumerable<KeyValuePair<string, IDetail>> ProvideDetails()
        {
            if (_reference == null)
            {
                return Enumerable.Empty<KeyValuePair<string, IDetail>>();
            }

            return _reference;
        }
    }
}