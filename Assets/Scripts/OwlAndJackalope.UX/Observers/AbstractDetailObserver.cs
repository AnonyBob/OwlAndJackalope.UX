using System;
using OwlAndJackalope.UX.Data;
using UnityEngine;

namespace OwlAndJackalope.UX.Observers
{
    [System.Serializable]
    public abstract class AbstractDetailObserver : IDisposable
    {
        public abstract IDetail Detail { get; set; }

        public bool IsSet => Detail != null;

        public object ObjectValue => Detail?.GetObject();

        public event Action OnChange;

        [SerializeField] 
        public string DetailName;
        
        private IReference _reference;
        private bool _suppressInitial;
        private bool _initialSetup;

        public AbstractDetailObserver() { }
        
        public AbstractDetailObserver(string detailName)
        {
            DetailName = detailName;
        }
        
        public void Initialize(IReference reference, Action changeHandler = null, bool suppressInitial = true)
        {
            _initialSetup = true;
            _suppressInitial = suppressInitial;
            if (changeHandler != null)
            {
                OnChange += changeHandler;    
            }

            _reference = reference;
            if (_reference != null)
            {
                _reference.VersionChanged += HandleReferenceChanged;
                HandleReferenceChanged();
            }
        }

        private void HandleReferenceChanged()
        {
            var newDetail = _reference.GetDetail(DetailName);
            if (Detail != null)
            {
                //If the detail hasn't changed then we don't need to bother updating.
                if (ReferenceEquals(newDetail, Detail))
                {
                    return;
                }
                
                Detail.VersionChanged -= HandleDetailChanged;
            }

            Detail = newDetail;
            if (Detail != null)
            {
                Detail.VersionChanged += HandleDetailChanged;
                if (!_initialSetup || !_suppressInitial)
                {
                    HandleDetailChanged();
                }
            }
            
            _initialSetup = false;
        }
        
        private void HandleDetailChanged()
        {
            if (Detail != null)
            {
                OnChange?.Invoke();
            }
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.VersionChanged -= HandleReferenceChanged;
            }

            if (Detail != null)
            {
                Detail.VersionChanged -= HandleDetailChanged;
            }

            OnChange = null;
        }
    }
}