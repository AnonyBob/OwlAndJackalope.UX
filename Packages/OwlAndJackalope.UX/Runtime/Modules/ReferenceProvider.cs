using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Modules
{
    [RequireComponent(typeof(ReferenceModule))]
    public abstract class ReferenceProvider : MonoBehaviour
    {
        public abstract IEnumerable<IDetail> ProvideReference();
    }
}