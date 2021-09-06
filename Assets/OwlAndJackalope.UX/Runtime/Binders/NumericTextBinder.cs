using System.Collections;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using TMPro;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Binders
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class NumericTextBinder : BaseDetailBinder
    {
        [SerializeField, DetailType(typeof(int), typeof(long), typeof(float), typeof(double))]
        private DetailObserver _number;

        [SerializeField]
        private float _numberChangeDuration;

        [SerializeField]
        private string _numberFormat;

        private TextMeshProUGUI _text;
        
        private Coroutine _routine;
        private DetailObserver<int> _intObserver;
        private DetailObserver<float> _floatObserver;
        private DetailObserver<double> _doubleObserver;
        private DetailObserver<long> _longObserver;

        private long? _previousLong;
        private double? _previousDouble;
        
        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _number.Initialize(_referenceModule.Reference, null);
            var selectedType = _number.Detail.GetObjectType();

            if (selectedType == typeof(int))
            {
                _intObserver = CreateDetail<int>();
            }
            else if (selectedType == typeof(float))
            {
                _floatObserver = CreateDetail<float>();
            }
            else if (selectedType == typeof(int))
            {
                _doubleObserver = CreateDetail<double>();
            }
            else if (selectedType == typeof(long))
            {
                _longObserver = CreateDetail<long>(); ;
            }
            
            UpdateNumber();
        }

        private void UpdateNumber()
        {
            if (_intObserver != null)
            {
                UpdateNumber(_intObserver.Value);
            }
            else if (_floatObserver != null)
            {
                UpdateNumber(_floatObserver.Value);
            }
            else if (_longObserver != null)
            {
                UpdateNumber(_longObserver.Value);
            }
            else if (_doubleObserver != null)
            {
                UpdateNumber(_doubleObserver.Value);
            }
        }

        private void UpdateNumber(long value)
        {
            if (_numberChangeDuration > 0 && gameObject.activeInHierarchy && _previousLong.HasValue)
            {
                if(_routine != null)
                    StopCoroutine(_routine);
                _routine = StartCoroutine(UpdateOverTime(value));
            }
            else
            {
                _text.text = value.ToString(_numberFormat);                
            }

            _previousLong = value;
        }

        private void UpdateNumber(double value)
        {
            if (_numberChangeDuration > 0 && gameObject.activeInHierarchy && _previousDouble.HasValue)
            {
                if(_routine != null)
                    StopCoroutine(_routine);
                _routine = StartCoroutine(UpdateOverTime(value));
            }
            else
            {
                _text.text = value.ToString(_numberFormat);                
            }

            _previousDouble = value;
        }

        private IEnumerator UpdateOverTime(long value)
        {
            var startingValue = _previousLong.HasValue ? _previousLong.Value : 0;
            var currentValue = startingValue;
            var time = 0.0;
            while (time < _numberChangeDuration)
            {
                currentValue = (long) (startingValue + (value - startingValue) * (time / _numberChangeDuration));
                _text.text = currentValue.ToString(_numberFormat); 
                
                time += Time.deltaTime;
                yield return null;
            }

            _text.text = value.ToString(_numberFormat); 
            _routine = null;
        }
        
        private IEnumerator UpdateOverTime(double value)
        {
            var startingValue = _previousDouble.HasValue ? _previousDouble.Value : 0;
            var currentValue = startingValue;
            var time = 0.0f;
            while (time < _numberChangeDuration)
            {
                currentValue = (startingValue + (value - startingValue) * (time / _numberChangeDuration));
                _text.text = currentValue.ToString(_numberFormat); 
                
                time += Time.deltaTime;
                yield return null;
            }

            _text.text = value.ToString(_numberFormat); 
            _routine = null;
        }

        private DetailObserver<T> CreateDetail<T>()
        {
            var observer = new DetailObserver<T>();
            observer.DetailName = _number.DetailName;
            observer.Initialize(_referenceModule.Reference, UpdateNumber);
            return observer;
        }

        protected override IEnumerable<AbstractDetailObserver> GetDetailObservers()
        {
            yield return _number;
            yield return _intObserver;
            yield return _floatObserver;
            yield return _doubleObserver;
            yield return _longObserver;
        }
    }
}