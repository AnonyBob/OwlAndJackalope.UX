using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Testing
{
    public class TestReferenceProvider : ReferenceProvider
    {
        public ReferenceTemplate Template;
        
        public override IReference ProvideReference()
        {
            return Template.Reference.ConvertToReference();
        }
    }
}