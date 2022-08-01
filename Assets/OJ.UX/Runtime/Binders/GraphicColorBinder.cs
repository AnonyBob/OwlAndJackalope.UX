using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace OJ.UX.Runtime.Binders
{
    public class GraphicColorBinder : AbstractDetailBinder
    {
        [SerializeField]
        private Observer<Color> _color;
        
        [SerializeField]
        private Graphic _graphic;

        private void Start()
        {
            if (_graphic == null)
            {
                _graphic = GetComponent<Graphic>();
            }
            _color.Initialize(HandleChange);
        }

        private void OnDestroy()
        {
            _color.Destroy();
        }

        private void HandleChange()
        {
            if (_graphic != null)
            {
                _graphic.color = _color.Value;
            }
        }
    }
}