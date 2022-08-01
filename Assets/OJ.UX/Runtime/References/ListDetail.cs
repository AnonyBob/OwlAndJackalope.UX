using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OJ.UX.Runtime.References
{
    public class ListDetail<TValue> : IMutableDetail<List<TValue>>, IList<TValue>
    {
        public event Action OnChanged;

        public List<TValue> Value
        {
            get => _value;
            set
            {
                if (!ReferenceEquals(_value, value))
                {
                    _value = value;
                    Version++;
                    
                    OnChanged?.Invoke();
                }
            }
        }

        public long Version { get; private set; }

        public int Count => _value?.Count ?? 0;

        public bool IsReadOnly => false;

        public TValue this[int index]
        {
            get => _value[index];
            set
            {
                _value[index] = value;
                Version++;
                
                OnChanged?.Invoke();
            }
        }

        private List<TValue> _value;

        public ListDetail()
        {
            _value = new List<TValue>();
        }

        public ListDetail(List<TValue> list, bool makeCopy = false)
        {
            if (makeCopy)
                _value = new List<TValue>(list);

            _value = list;
        }

        public ListDetail(IEnumerable<TValue> collection)
        {
            _value = new List<TValue>(collection);
        }

        public void Add(TValue item)
        {
            if (_value == null)
            {
                _value = new List<TValue>();
            }
            
            _value.Add(item);
            Version++;
            
            OnChanged?.Invoke();
        }

        public void Clear()
        {
            if (_value != null)
            {
                _value.Clear();
                Version++;
                
                OnChanged?.Invoke();
            }
        }

        public bool Contains(TValue item)
        {
            return _value?.Contains(item) ?? false;
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _value?.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            if (_value != null)
            {
                if (_value.Remove(item))
                {
                    Version++;
                    OnChanged?.Invoke();

                    return true;
                }
            }

            return false;
        }

        public int IndexOf(TValue item)
        {
            return _value?.IndexOf(item) ?? -1;
        }

        public void Insert(int index, TValue item)
        {
            if (_value != null)
            {
                _value.Insert(index, item);
                Version++;
                
                OnChanged?.Invoke();
            }
        }

        public void RemoveAt(int index)
        {
            if (_value != null)
            {
                _value.RemoveAt(index);
                Version++;
                
                OnChanged?.Invoke();
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _value?.GetEnumerator() ?? Enumerable.Empty<TValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        object IDetail.Value => Value;
    }
}