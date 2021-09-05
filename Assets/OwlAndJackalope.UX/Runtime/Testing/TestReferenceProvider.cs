using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Testing
{
    public class TestReferenceProvider : ReferenceProvider
    {
        public ReferenceTemplate Template;
        
        public override IEnumerable<IDetail> ProvideReference()
        {
            return Template.Reference.ConvertToReference();
        }

        [ContextMenu("Test Change")]
        public void TestChange()
        {
            GetComponent<ReferenceModule>().AddDetails(ProvideReference());
        }
    }
}