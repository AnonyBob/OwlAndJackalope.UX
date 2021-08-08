using OwlAndJackalope.UX.Data;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    [RequireComponent(typeof(ReferenceModule))]
    public abstract class ReferenceProvider : MonoBehaviour
    {
        public abstract IReference ProvideReference();
    }
}