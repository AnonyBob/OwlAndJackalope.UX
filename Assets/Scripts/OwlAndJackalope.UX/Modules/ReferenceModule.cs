
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using OwlAndJackalope.UX.Modules.Initializers;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules
{
    /// <summary>
    /// The module on an experience responsible for defining the details that drive the
    /// state and related actions.
    /// </summary>
    [System.Serializable]
    public class ReferenceModule
    {
        [SerializeField]
        private BaseSerializedReference _reference;
        
        public IReference GetReference()
        {
            return _reference.ConvertToReference();
        }
    }
}