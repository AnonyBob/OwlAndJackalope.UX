using System;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
     [System.Serializable]
    public class Observer : IDisposable
    {
        public IDetail Detail { get; private set; }

        public bool IsSet => Detail != null;

        public object Value => Detail?.Value;

        [SerializeField]
        private string _detailName;
        
        [SerializeField]
        private ReferenceModule _referenceModule;

        private event Action _onChange;
        private IReference _reference;

        public void Initialize(Action changeHandler = null, bool suppressInitial = false)
        {
            if (changeHandler != null)
            {
                _onChange += changeHandler;
            }

            if (_reference == null && _referenceModule != null)
            {
                _reference = _referenceModule.Reference;
                _reference.OnChanged += HandleReferenceChanged;
                HandleReferenceChanged(suppressInitial);
            }
        }

        public TValue GetValue<TValue>()
        {
            if (Detail is IDetail<TValue> detailAsType)
                return detailAsType.Value;

            return default;
        }

        private void HandleReferenceChanged()
        {
            HandleReferenceChanged(false);
        }
        
        private void HandleReferenceChanged(bool suppressInitial)
        {
            var newDetail = _reference.GetDetail(_detailName);
            if (Detail != null)
            {
                //Do nothing if this is the existing detail.
                if (ReferenceEquals(newDetail, Detail))
                {
                    return;
                }

                Detail.OnChanged -= HandleDetailChanged;
            }

            Detail = newDetail;
            if (Detail != null)
            {
                Detail.OnChanged += HandleDetailChanged;
                if (!suppressInitial)
                {
                    HandleDetailChanged();
                }
            }
        }

        private void HandleDetailChanged()
        {
            if(Detail != null)
                _onChange?.Invoke();
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.OnChanged -= HandleReferenceChanged;
            }

            _onChange = null;
        }
    }
    
    [System.Serializable]
    public class Observer<TValue> : IDisposable
    {
        public IDetail<TValue> Detail { get; private set; }

        public bool IsSet => Detail != null;

        public bool CanMutate => _mutableDetail != null;

        public TValue Value
        {
            get => Detail != null ? Detail.Value : default(TValue);
            set
            {
                if (_mutableDetail != null)
                    _mutableDetail.Value = value;
            }
        } 

        [SerializeField]
        private string _detailName;
        
        [SerializeField]
        private ReferenceModule _referenceModule;

        private event Action _onChange;
        
        private IReference _reference;
        private IMutableDetail<TValue> _mutableDetail;

        public void Initialize(Action changeHandler = null, bool suppressInitial = false)
        {
            if (changeHandler != null)
            {
                _onChange += changeHandler;
            }

            if (_reference == null && _referenceModule != null)
            {
                _reference = _referenceModule.Reference;
                _reference.OnChanged += HandleReferenceChanged;
                HandleReferenceChanged(suppressInitial);
            }
        }

        private void HandleReferenceChanged()
        {
            HandleReferenceChanged(false);
        }
        
        private void HandleReferenceChanged(bool suppressInitial)
        {
            var newDetail = _reference.GetDetail<TValue>(_detailName);
            if (Detail != null)
            {
                //Do nothing if this is the existing detail.
                if (ReferenceEquals(newDetail, Detail))
                {
                    return;
                }

                Detail.OnChanged -= HandleDetailChanged;
            }

            Detail = newDetail;
            _mutableDetail = Detail as IMutableDetail<TValue>;
            
            if (Detail != null)
            {
                Detail.OnChanged += HandleDetailChanged;
                if (!suppressInitial)
                {
                    HandleDetailChanged();
                }
            }
        }

        private void HandleDetailChanged()
        {
            if(Detail != null)
                _onChange?.Invoke();
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.OnChanged -= HandleReferenceChanged;
            }

            _onChange = null;
        }
    }
}