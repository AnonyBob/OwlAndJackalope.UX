using System;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
     [System.Serializable]
    public class Observer : IDisposable
    {
        public IDetail ObjectDetail => _objectDetail;

        public bool IsSet => ObjectDetail != null;

        [SerializeField] protected string _detailName;
        [SerializeField] protected ReferenceModule _referenceModule;

        protected event Action _onChange;
        protected IReference _reference;
        protected IDetail _objectDetail;

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

        public void RemoveHandler(Action changeHandler)
        {
            _onChange -= changeHandler;
        }

        public TValue GetValue<TValue>()
        {
            if (_objectDetail is IDetail<TValue> detailAsType)
                return detailAsType.Value;

            return default;
        }
        
        protected virtual void HandleReferenceChanged(bool suppressInitial)
        {
            var newDetail = _reference.GetDetail(_detailName);
            if (_objectDetail != null)
            {
                //Do nothing if this is the existing detail.
                if (ReferenceEquals(newDetail, _objectDetail))
                {
                    return;
                }

                _objectDetail.OnChanged -= HandleDetailChanged;
            }

            _objectDetail = newDetail;
            if (_objectDetail != null)
            {
                _objectDetail.OnChanged += HandleDetailChanged;
                if (!suppressInitial)
                {
                    HandleDetailChanged();
                }
            }
        }

        protected void HandleReferenceChanged()
        {
            HandleReferenceChanged(false);
        }

        protected void HandleDetailChanged()
        {
            if(_objectDetail != null)
                _onChange?.Invoke();
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.OnChanged -= HandleReferenceChanged;
            }
            
            if (_objectDetail != null)
            {
                _objectDetail.OnChanged -= HandleDetailChanged;
            }

            _onChange = null;
        }
    }
    
    [Serializable]
    public class Observer<TValue> : Observer
    {
        public IDetail<TValue> Detail => _valueDetail;

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

        private IMutableDetail<TValue> _mutableDetail;
        private IDetail<TValue> _valueDetail;

        protected override void HandleReferenceChanged(bool suppressInitial)
        {
            var newDetail = _reference.GetDetail<TValue>(_detailName);
            if (_objectDetail != null)
            {
                //Do nothing if this is the existing detail.
                if (ReferenceEquals(newDetail, _objectDetail))
                {
                    return;
                }

                _objectDetail.OnChanged -= HandleDetailChanged;
            }

            _objectDetail = newDetail;
            _valueDetail = _objectDetail as IDetail<TValue>;
            _mutableDetail = _objectDetail as IMutableDetail<TValue>;
            
            if (_objectDetail != null)
            {
                _objectDetail.OnChanged += HandleDetailChanged;
                if (!suppressInitial)
                {
                    HandleDetailChanged();
                }
            }
        }
    }
}