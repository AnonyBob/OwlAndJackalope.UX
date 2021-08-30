using System.Collections.Generic;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Testing
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