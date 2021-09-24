using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.DetailBinders.Appliers
{
    public class ColorDetailApplier : BaseDetailApplier
    {
        [SerializeField, DetailType(typeof(Color))]
        private MutableDetailObserver<Color> _color;

        [SerializeField]
        private Color[] _providedColors;
        
        private void Start()
        {
            _color.Initialize(_referenceModule.Reference);
        }

        public void SetUsingProvidedColorIndex(int index)
        {
            if (!IsReadyToSet(_color))
            {
                return;
            }
            
            if (index >= 0 && _providedColors.Length > index)
            {
                _color.Value = _providedColors[index];    
            }
            else
            {
                Debug.LogError($"{name} doesn't have a color defined for: {index}");
            }
        }
        
        public void SetColor(Color color)
        {
            if (!IsReadyToSet(_color))
            {
                return;
            }
            _color.Value = color;
        }
        
        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _color;
        }
    }
}