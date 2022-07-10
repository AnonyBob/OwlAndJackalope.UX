using System;
using OJ.UX.Runtime.Binding;
using TMPro;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class TestBinder : AbstractDetailBinder
    {
        [SerializeField]
        private Observer<int> _intBinder;

        [SerializeField, ObserveDetails(typeof(string), typeof(double))]
        private Observer _stringBinder;

        [SerializeField]
        private TextMeshProUGUI _text;
        
        private void Start()
        {
            _intBinder.Initialize(HandleChange, true);
            _stringBinder.Initialize(HandleChange);
        }

        private void HandleChange()
        {
            _text.text = $"{_intBinder.Value} -- {_stringBinder.GetValue<string>()}";
        }

        private void OnDestroy()
        {
            _intBinder.Dispose();
            _stringBinder.Dispose();
        }
    }
}