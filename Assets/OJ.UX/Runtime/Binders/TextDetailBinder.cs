using System;
using System.Linq;
using OJ.UX.Runtime.Binding;
using TMPro;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class TextDetailBinder : AbstractDetailBinder
    {
        [SerializeField]
        private TextMeshProUGUI _textField;
        
        [SerializeField]
        private bool _useDefaultString;

        [SerializeField]
        private Observer<string> _defaultStringObserver;

        [SerializeField]
        private string _defaultString;

        [SerializeField, ObserveDetails(typeof(string), typeof(int),
             typeof(long), typeof(float), typeof(double), typeof(bool), 
             typeof(Enum), typeof(TimeSpan), typeof(DateTime))]
        private Observer[] _stringArgumentObservers;
        
        private FormattingProvider _formattingProvider;

        private void Start()
        {
            _formattingProvider = GetComponent<FormattingProvider>();
            if (_textField == null)
            {
                _textField = GetComponent<TextMeshProUGUI>();
            }
            
            _defaultStringObserver.Initialize(UpdateText, true);
            foreach (var observer in _stringArgumentObservers)
            {
                observer.Initialize(UpdateText, true);
            }

            UpdateText();
        }

        private void OnDestroy()
        {
            _defaultStringObserver.Destroy();
            foreach (var observer in _stringArgumentObservers)
            {
                observer.Destroy();
            }
        }
        
        private void UpdateText()
        {
            if (_textField == null)
                return;

            var textString = _useDefaultString ? _defaultString : _defaultStringObserver.Value;
            if (_stringArgumentObservers.Length > 0 && !string.IsNullOrEmpty(textString))
            {
                try
                {
                    if (_formattingProvider != null)
                    {
                        textString = string.Format(_formattingProvider.GetFormatter(), textString,
                            _stringArgumentObservers.Select(x => x.ObjectDetail.Value).ToArray());
                    }
                    else
                    {
                        textString = string.Format(textString,
                            _stringArgumentObservers.Select(x => x.ObjectDetail.Value).ToArray());
                    }
                }
                catch (FormatException)
                {
                    Debug.LogError($"{textString} is in a bad format");
                }
            }
            
            _textField.SetText(textString);
        }
    }
}