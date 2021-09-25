using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    [RequireComponent(typeof(Image))]
    public class SpriteBinder : BaseDetailBinder
    {
        [SerializeField, DetailType(typeof(Sprite))]
        private DetailObserver<Sprite> _observer;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private bool _useOverride;
        
        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            _observer.Initialize(_referenceModule.Reference, Handle, false);
        }

        private void Handle()
        {
            if (_image != null)
            {
                if (_useOverride)
                {
                    _image.overrideSprite = _observer.Value;
                }
                else
                {
                    _image.sprite = _observer.Value;    
                }
            }
        }
        
        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _observer;
        }
    }
}