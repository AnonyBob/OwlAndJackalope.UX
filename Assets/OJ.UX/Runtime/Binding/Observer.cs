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

        [SerializeField] protected string _detailName;
        [SerializeField] protected ReferenceModule _referenceModule;

        protected event Action _onChange;
        protected IReference _reference;

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
            if (Detail is IDetail<TValue> detailAsType)
                return detailAsType.Value;

            return default;
        }
        
        protected virtual void HandleReferenceChanged(bool suppressInitial)
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

        protected void HandleReferenceChanged()
        {
            HandleReferenceChanged(false);
        }

        protected void HandleDetailChanged()
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
            
            if (Detail != null)
            {
                Detail.OnChanged -= HandleDetailChanged;
            }

            _onChange = null;
        }
    }
    
    [Serializable]
    public class Observer<TValue> : Observer
    {
        public new IDetail<TValue> Detail { get; private set; }

        public new bool IsSet => Detail != null;

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

        protected override void HandleReferenceChanged(bool suppressInitial)
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
    }
}