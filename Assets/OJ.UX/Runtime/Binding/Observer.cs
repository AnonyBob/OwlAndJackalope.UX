using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    [System.Serializable]
    public class Observer
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

        public void Destroy()
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
                else
                    Debug.LogWarning($"{_detailName} is not mutable.");
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
    
    [Serializable]
    public class ListObserver<TValue> : Observer, IList<TValue>
    {
        public IDetail<List<TValue>> Detail => _valueDetail;

        public ListDetail<TValue> ListDetail => _mutableDetail;

        public bool CanMutate => _mutableDetail != null;

        public List<TValue> Value
        {
            get => Detail != null ? Detail.Value : null;
            set
            {
                if (_mutableDetail != null)
                    _mutableDetail.Value = value;
                else
                    Debug.LogWarning($"{_detailName} is not mutable.");
            }
        }

        public int Count => _valueDetail?.Value?.Count ?? 0;
        
        public bool IsReadOnly => !CanMutate;

        private ListDetail<TValue> _mutableDetail;
        private IDetail<List<TValue>> _valueDetail;
        
        public TValue this[int index]
        {
            get => _valueDetail?.Value != null 
                ? _valueDetail.Value[index] 
                : throw new ArgumentOutOfRangeException("List is not provided");
            set
            {
                if (!CanMutate)
                {
                    throw new InvalidOperationException("List is not mutable");
                }

                _mutableDetail[index] = value;
            }
        }
        
        
        public void Add(TValue item)
        {
            if (!CanMutate)
            {
                throw new InvalidOperationException("List is not mutable");
            }
            
            _mutableDetail.Add(item);
        }

        public void Clear()
        {
            if (!CanMutate)
            {
                throw new InvalidOperationException("List is not mutable");
            }
            
            _mutableDetail.Clear();
        }

        public bool Contains(TValue item)
        {
            return _valueDetail?.Value?.Contains(item) ?? false;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _valueDetail?.Value?.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            if (!CanMutate)
            {
                throw new InvalidOperationException("List is not mutable");
            }
            
            return _mutableDetail.Remove(item);
        }
        
        public int IndexOf(TValue item)
        {
            return _valueDetail?.Value?.IndexOf(item) ?? -1;
        }

        public void Insert(int index, TValue item)
        {
            if (!CanMutate)
            {
                throw new InvalidOperationException("List is not mutable");
            }
            
            _mutableDetail.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (!CanMutate)
            {
                throw new InvalidOperationException("List is not mutable");
            }
            
            _mutableDetail.RemoveAt(index);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _valueDetail?.Value?.GetEnumerator() ?? Enumerable.Empty<TValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
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
            _valueDetail = _objectDetail as IDetail<List<TValue>>;
            _mutableDetail = _objectDetail as ListDetail<TValue>;
            
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