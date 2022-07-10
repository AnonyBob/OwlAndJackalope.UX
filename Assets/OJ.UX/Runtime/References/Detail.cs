using System;
using System.Collections.Generic;

namespace OJ.UX.Runtime.References
{
    public class Detail<TValue> : IMutableDetail<TValue>
    {
        public event Action OnChanged;

        public TValue Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<TValue>.Default.Equals(_value, value))
                {
                    _value = value;
                    Version++;
                    
                    OnChanged?.Invoke();
                }
            }
        }
        
        public long Version { get; private set; }

        private TValue _value;
        private readonly IEqualityComparer<TValue> _comparer;

        public Detail() : this(default(TValue), null)
        {
        }

        public Detail(TValue initialValue) : this(initialValue, null)
        {
        }

        public Detail(TValue value, IEqualityComparer<TValue> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        object IDetail.Value => Value;
    }
}