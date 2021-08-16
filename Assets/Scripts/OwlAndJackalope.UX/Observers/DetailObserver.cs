using System;
using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.Observers
{
    public interface IDetailObserver : IDisposable
    {
        
    }
    
    public class DetailObserver : IDetailObserver
    {
        public IDetail Detail => _detail;

        public bool IsSet => _detail != null;
        
        public object ObjectValue
        {
            get
            {
                if (_detail != null)
                {
                    return _detail.GetObject();
                }
                return default;
            }
        }
        
        private readonly string _detailName;
        private readonly IReference _reference;
        private readonly Action _changeAction;
        private readonly bool _suppressInitial;

        private IDetail _detail;
        private bool _initialSetup = true;

        public DetailObserver(string detailName, IReference reference, Action changeAction = null, bool suppressInitial = true)
        {
            _detailName = detailName;
            _reference = reference;
            _changeAction = changeAction;
            _suppressInitial = suppressInitial;
            _reference.VersionChanged += HandleReferenceChanged;
            HandleReferenceChanged();
        }

        private void HandleReferenceChanged()
        {
            var newDetail = _reference.GetDetail(_detailName);
            if (_detail != null)
            {
                //If the detail hasn't changed then we don't need to bother updating.
                if (ReferenceEquals(newDetail, _detail))
                {
                    return;
                }
                
                _detail.VersionChanged -= HandleDetailChanged;
            }
            
            _detail = newDetail;
            if (_detail != null)
            {
                _detail.VersionChanged += HandleDetailChanged;
                if (!_initialSetup || !_suppressInitial)
                {
                    _initialSetup = false;
                    HandleDetailChanged();
                }
            }
        }
        
        private void HandleDetailChanged()
        {
            if (_detail != null)
            {
                _changeAction?.Invoke();
            }
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.VersionChanged -= HandleReferenceChanged;
            }

            if (_detail != null)
            {
                _detail.VersionChanged -= HandleDetailChanged;
            }
        }
    }
    
    public class DetailObserver<T> : IDetailObserver
    {
        public bool IsSet => _detail != null;
        
        public T Value
        {
            get
            {
                if (_detail != null)
                {
                    return _detail.GetValue();
                }
                return default;
            }
        }
        
        private readonly string _detailName;
        private readonly IReference _reference;
        private readonly Action<T> _changeAction;
        private readonly bool _suppressInitial;

        private IDetail<T> _detail;
        private bool _initialSetup = true;
        
        public DetailObserver(string detailName, IReference reference, Action<T> changeAction = null, bool suppressInitial = true)
        {
            _detailName = detailName;
            _reference = reference;
            _changeAction = changeAction;
            _suppressInitial = suppressInitial;
            _reference.VersionChanged += HandleReferenceChanged;
            HandleReferenceChanged();
        }

        private void HandleReferenceChanged()
        {
            var newDetail = _reference.GetDetail<T>(_detailName);
            if (_detail != null)
            {
                //If the detail hasn't changed then we don't need to bother updating.
                if (ReferenceEquals(newDetail, _detail))
                {
                    return;
                }
                
                _detail.VersionChanged -= HandleDetailChanged;
            }
            
            _detail = newDetail;
            if (_detail != null)
            {
                _detail.VersionChanged += HandleDetailChanged;
                if (!_initialSetup || !_suppressInitial)
                {
                    _initialSetup = false;
                    HandleDetailChanged();
                }
            }
        }
        
        private void HandleDetailChanged()
        {
            if (_detail != null)
            {
                _changeAction?.Invoke(_detail.GetValue());
            }
        }

        public void Dispose()
        {
            if (_reference != null)
            {
                _reference.VersionChanged -= HandleReferenceChanged;
            }

            if (_detail != null)
            {
                _detail.VersionChanged -= HandleDetailChanged;
            }
        }
    }
}