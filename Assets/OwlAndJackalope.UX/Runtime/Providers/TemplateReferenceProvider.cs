using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Providers
{
    public class TemplateReferenceProvider : ReferenceProvider
    {
        [SerializeField]
        private ReferenceTemplate _template;

        public override IEnumerable<IDetail> ProvideReference()
        {
            if (_template != null)
            {
                return _template.Reference.ConvertToReference();
            }
            
            return new BaseReference();
        }

        [ContextMenu("Force Set")]
        private void ForceTemplateChange()
        {
            if (_template != null)
            {
                GetComponent<ReferenceModule>().Reference = _template.Reference.ConvertToReference();
            }
        }
    }
}