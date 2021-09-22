using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;
using UnityEngine.UI;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    public class GraphicColorBinder : BaseDetailBinder
    {
        [SerializeField, DetailType(typeof(Color))]
        private DetailObserver<Color> _observer;

        [SerializeField]
        private Graphic _graphic;

        private void Start()
        {
            if (_graphic == null)
            {
                _graphic = GetComponent<Graphic>();
            }
            
            _observer.Initialize(_referenceModule.Reference, Handle, false);
        }

        private void Handle()
        {
            if (_graphic != null)
            {
                _graphic.color = _observer.Value;
            }   
        }
        
        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _observer;
        }
    }
}