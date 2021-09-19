using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OwlAndJackalope.UX.Runtime.Data
{
    public class BaseCollectionDetail<T> : IMutableCollectionDetail<T>
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


        public int Count => _value?.Count ?? 0;
        public bool IsReadOnly => false;
        
        public T this[int index]
        {
            get => _value[index];
            set
            {
                _value[index] = value;
                Version++;
            }
        }

        private List<T> _value;
        private long _version;
        
        public BaseCollectionDetail(string name, IEnumerable<T> initialValue, bool duplicate)
        {
            Name = name;
            if (initialValue != null)
            {
                //Allow us the option to duplicate the list so that it can
                var initialValueAsList = initialValue as List<T>;
                if (duplicate || initialValueAsList == null)
                {
                    _value = new List<T>();
                    _value.AddRange(initialValue);
                }
                else
                {
                    _value = initialValueAsList;
                }
            }
        }

        public List<T> GetValue()
        {
            return _value;
        }

        public bool SetValue(List<T> value)
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
            return SetValue((List<T>) obj);
        }

        public Type GetObjectType()
        {
            return typeof(List<T>);
        }

        public Type GetItemType()
        {
            return typeof(T);
        }

        public void Add(T item)
        {
            if (_value != null)
            {
                _value.Add(item);
                Version++;
            }
        }

        public void Clear()
        {
            if (_value != null)
            {
                _value.Clear();
                Version++;
            }
        }

        public bool Contains(T item)
        {
            return _value?.Contains(item) ?? false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _value?.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_value != null && _value.Remove(item))
            {
                Version++;
                return true;
            }

            return false;
        }
        
        public int IndexOf(T item)
        {
            return _value?.IndexOf(item) ?? -1;
        }

        public void Insert(int index, T item)
        {
            if (_value != null)
            {
                _value.Insert(index, item);
                Version++;
            }
        }

        public void RemoveAt(int index)
        {
            if (_value != null)
            {
                _value.RemoveAt(index);
                Version++;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _value?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}