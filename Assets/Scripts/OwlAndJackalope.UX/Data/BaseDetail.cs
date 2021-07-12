using System;
using System.Collections.Generic;

namespace OwlAndJackalope.UX.Data
{
    /// <summary>
    /// A simple mutable detail that contains a single piece of information. Its version will change
    /// whenever the data stored within is updated.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class BaseDetail<TValue> : IMutableDetail<TValue>
    {
        public string Name { get; }
        
        public event Action VersionChanged;

        public long Version
        {
            get => _version;
            private set
            {
                _version = value;
                VersionChanged?.Invoke();
            }
        }

        private TValue _internalValue;
        private long _version;

        public BaseDetail(string name, TValue initialValue = default)
        {
            Name = name;
            _internalValue = initialValue;
        }
        
        public TValue GetValue()
        {
            return _internalValue;
        }

        public bool SetValue(TValue value)
        {
            if (!_internalValue.Equals(value))
            {
                _internalValue = value;
                Version++;
                return true;
            }

            return false;
        }
        
        public object GetObject()
        {
            return GetValue();
        }

        public bool SetObject(object obj)
        {
            return SetValue((TValue) obj);
        }

        public override string ToString()
        {
            return $"{Name}-{typeof(TValue).Name}: {_internalValue}";
        }
    }
}