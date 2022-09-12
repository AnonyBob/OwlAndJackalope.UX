using System;

namespace OJ.UX.Runtime.References
{
    public class TransformDetail<TOriginal, TValue> : IDetail<TValue>
    {
        public TValue Value
        {
            get
            {
                if (_lastVersion != _originalValue.Version)
                {
                    _cachedValue = _transform.Invoke(_originalValue.Value);
                    _lastVersion = _originalValue.Version;
                }

                return _cachedValue;
            }
        }

        public Type ValueType => typeof(TValue);

        public long Version => _originalValue.Version;
        
        public event Action OnChanged;
        
        private readonly Func<TOriginal, TValue> _transform;
        private readonly IDetail<TOriginal> _originalValue;
        
        private TValue _cachedValue;
        private long _lastVersion = -1;
        
        public TransformDetail(Func<TOriginal, TValue> transform, IDetail<TOriginal> originalValue)
        {
            _transform = transform;
            _originalValue = originalValue;

            _originalValue.OnChanged += HandleChange;
        }

        public void Destroy()
        {
            _originalValue.OnChanged -= HandleChange;
        }

        private void HandleChange()
        {
            OnChanged?.Invoke();
        }

        object IDetail.Value => Value;
    }

    public class TransformDetail<TOriginalOne, TOriginalTwo, TValue> : IDetail<TValue>
    {
        public TValue Value
        {
            get
            {
                if (_lastVersionOne != _originalOne.Version || _lastVersionTwo != _originalTwo.Version)
                {
                    _cachedValue = _transform.Invoke(_originalOne.Value, _originalTwo.Value);
                    _lastVersionOne = _originalOne.Version;
                    _lastVersionTwo = _originalTwo.Version;
                }

                return _cachedValue;
            }
        }
        
        public Type ValueType => typeof(TValue);

        public long Version => _version;
        
        public event Action OnChanged;
        
        private readonly Func<TOriginalOne, TOriginalTwo, TValue> _transform;
        private readonly IDetail<TOriginalOne> _originalOne;
        private readonly IDetail<TOriginalTwo> _originalTwo;
        
        private TValue _cachedValue;
        private long _lastVersionOne = -1;
        private long _lastVersionTwo = -1;
        private long _version = 0;
        
        public TransformDetail(Func<TOriginalOne, TOriginalTwo, TValue> transform, IDetail<TOriginalOne> originalOne, IDetail<TOriginalTwo> originalTwo)
        {
            _transform = transform;
            _originalOne = originalOne;
            _originalTwo = originalTwo;

            _originalOne.OnChanged += HandleChange;
            _originalTwo.OnChanged += HandleChange;
        }

        public void Destroy()
        {
            _originalOne.OnChanged -= HandleChange;
            _originalTwo.OnChanged -= HandleChange;
        }

        private void HandleChange()
        {
            _version++;
            OnChanged?.Invoke();
        }

        object IDetail.Value => Value;
    }
}