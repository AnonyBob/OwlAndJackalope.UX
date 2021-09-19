using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OwlAndJackalope.UX.Runtime.Data
{
    public class BaseMapDetail<TKey, TValue> : IMutableMapDetail<TKey, TValue>
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


        public ICollection<TKey> Keys => _value.Keys;
        public ICollection<TValue> Values => _value.Values;
        public int Count => _value?.Count ?? 0;
        public bool IsReadOnly => false;
        
        public TValue this[TKey key]
        {
            get => _value[key];
            set
            {
                _value[key] = value;
                Version++;
            }
        }

        private Dictionary<TKey, TValue> _value;
        private long _version;

        public BaseMapDetail(string name, Dictionary<TKey, TValue> initialValue, bool duplicate)
        {
            Name = name;
            if (duplicate)
            {
                _value = new Dictionary<TKey, TValue>();
                if (initialValue != null)
                {
                    foreach (var kvp in initialValue)
                    {
                        _value[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                _value = initialValue;
            }
        }
        
        public Dictionary<TKey, TValue> GetValue()
        {
            return _value;
        }

        public bool SetValue(Dictionary<TKey, TValue> value)
        {
            if (!ReferenceEquals(_value, value))
            {
                _value = value;
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
            return SetValue((Dictionary<TKey, TValue>) obj);
        }

        public Type GetObjectType()
        {
            return typeof(Dictionary<TKey, TValue>);
        }

        public (Type KeyType, Type ValueType) GetItemType()
        {
            return (typeof(TKey), typeof(TValue));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }
        
        public void Add(TKey key, TValue value)
        {
            if (_value != null)
            {
                _value.Add(key, value);
                Version++;
            }
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }
        
        public bool Remove(TKey key)
        {
            if (_value != null && _value.Remove(key))
            {
                Version++;
                return true;
            }

            return false;
        }

        public void Clear()
        {
            if (_value != null)
            {
                _value.Clear();
                Version++;
            }
        }
        
        public bool ContainsKey(TKey key)
        {
            return _value?.ContainsKey(key) ?? false;
        }
        
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out var value))
            {
                return item.Value.Equals(value);
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_value != null)
            {
                return TryGetValue(key, out value);
            }

            value = default(TValue);
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (array.Length - index < Count)
            {
                throw new ArgumentException("Cannot copy to array as count is too small.");
            }
            foreach (var kvp in _value)
            {
                array[index++] = kvp;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _value?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}