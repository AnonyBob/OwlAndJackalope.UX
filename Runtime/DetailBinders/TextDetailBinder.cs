using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Observers;
using TMPro;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDetailBinder : BaseDetailBinder
    {
        [SerializeField]
        private bool _useDefaultString;
        
        [SerializeField, DetailType(typeof(string))]
        private DetailObserver<string> _baseStringObserver;

        [SerializeField]
        private string _defaultString;
        
        [SerializeField, DetailType(typeof(string), typeof(int),
              typeof(long), typeof(double), typeof(bool), typeof(Enum))]
        private DetailObserver[] _stringArgumentObservers;
        
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            var reference = _referenceModule.Reference;
            
            _baseStringObserver.Initialize(reference, UpdateText);
            foreach (var observer in _stringArgumentObservers)
            {
                observer.Initialize(reference, UpdateText);
            }
            
            UpdateText();
        }

        private void UpdateText()
        {
            if (_text != null)
            {
                var formatText = _useDefaultString ? _defaultString : _baseStringObserver.Value;
                if (_stringArgumentObservers?.Length > 0 && !string.IsNullOrEmpty(formatText))
                {
                    try
                    {
                        formatText = string.Format(formatText, _stringArgumentObservers.Select(x => x.ObjectValue).ToArray());
                    }
                    catch (FormatException)
                    {
                        Debug.LogWarning($"{formatText} is in a bad format");
                    }
                }
                
                _text.SetText(formatText);
            }
        }

        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _baseStringObserver;
            foreach (var observer in _stringArgumentObservers)
            {
                yield return observer;
            }
        }
    }
}