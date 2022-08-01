using System.Collections.Generic;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.References.Serialized;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    public class TemplateDetailsProvider : DetailsProvider
    {
        [SerializeField]
        private SerializedReferenceTemplate _template;
        
        public override IEnumerable<KeyValuePair<string, IDetail>> ProvideDetails()
        {
            return _template.CreateReference();
        }

        [ContextMenu("Set Reference")]
        private void SetReference()
        {
            if(Application.isPlaying)
                GetComponent<ReferenceModule>().Reference = _template.CreateReference();
        }
    }
}