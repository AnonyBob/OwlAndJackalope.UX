using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageTextureBinder : BaseDetailBinder
    {
        [SerializeField, DetailType(typeof(Texture2D))]
        private DetailObserver<Texture2D> _observer;

        [SerializeField]
        private RawImage _image;
        
        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<RawImage>();
            }
            _observer.Initialize(_referenceModule.Reference, Handle, false);
        }

        private void Handle()
        {
            if (_image != null)
            {
                _image.texture = _observer.Value;
            }
        }
        
        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _observer;
        }
    }
}