using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Modules.Initializers
{
    /// <summary>
    /// Base reference provider that provides the fallback template. Can be overridden to
    /// provide functionality required to retrieve initial reference. Upon failure fallback
    /// will continue to be used.
    ///
    /// Useful for local testing whenever the surrounding game may not be loaded to provide the
    /// actual reference.
    /// </summary>
    [RequireComponent(typeof(Experience))]
    public class ReferenceProvider : MonoBehaviour, IReferenceProvider
    {
        [SerializeField]
        private ReferenceTemplate _fallback;

        private void OnEnable()
        {
            GetComponent<Experience>().Reference = GetReference();
        }
        
        public IReference GetReference()
        {
            return GetOverrideReference() ?? _fallback.Reference.ConvertToReference();
        }

        protected virtual IReference GetOverrideReference()
        {
            return null;
        }
    }
}