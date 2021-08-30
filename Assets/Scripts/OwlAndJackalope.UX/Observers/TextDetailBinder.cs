using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace OwlAndJackalope.UX.Observers
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

        private void OnDestroy()
        {
            _baseStringObserver?.Dispose();
            for (var i = 0; i < _stringArgumentObservers.Length; ++i)
            {
                _stringArgumentObservers[i].Dispose();
            }
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

        protected override int UpdateDetailNames(string previousName, string newName)
        {
            var sum = UpdateDetailName(_baseStringObserver, previousName, newName);
            for (var i = 0; i < _stringArgumentObservers.Length; ++i)
            {
                sum += UpdateDetailName(_stringArgumentObservers[i], previousName, newName);
            }
            return sum;
        }
    }
}