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
        public IReference Reference
        {
            get => _runtimeReference;
            set
            {
                if (_runtimeReference == null)
                {
                    _runtimeReference = new BaseReference(_reference.ConvertToReference());
                }

                _runtimeReference.AddDetails(value);
            }
        }
        
        [SerializeField]
        private BaseSerializedReference _reference;
        private IMutableReference _runtimeReference;

        public void Initialize(ExperienceReferenceProvider provider)
        {
            _runtimeReference = new BaseReference(_reference.ConvertToReference());
            
            if (provider != null)
            {
                var providedReference = provider.GetReferenceForExperience();
                if (providedReference != null)
                {
                    _runtimeReference.AddDetails(providedReference);    
                }
                
            }
        }
    }
}