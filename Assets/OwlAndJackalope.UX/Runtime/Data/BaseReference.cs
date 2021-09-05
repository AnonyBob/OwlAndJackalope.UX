using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OwlAndJackalope.UX.Runtime.Data
{
    /// <summary>
    /// Simple base reference that can be initialized with a collection of starting details.
    /// Details are easily accessed via their name and the version of the BaseReference will only
    /// change if details are added or removed. 
    /// </summary>
    public sealed class BaseReference : IMutableReference
    {
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

        private long _version;
        private readonly Dictionary<string, IDetail> _details = new Dictionary<string, IDetail>();

        public BaseReference(IEnumerable<IDetail> startingDetails)
        {
            if (startingDetails != null)
            {
                foreach (var detail in startingDetails)
                {
                    if (detail != null)
                    {
                        _details[detail.Name] = detail;    
                    }
                } 
            }
        }

        public BaseReference()
        {
        }

        public IDetail GetDetail(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _details.TryGetValue(name, out var detail);
                return detail;
            }

            return null;
        }
        
        public IDetail<TValue> GetDetail<TValue>(string name)
        {
            return GetDetail(name) as IDetail<TValue>;
        }

        public bool AddDetail(IDetail detail, bool overwrite = true)
        {
            var added = InternalAdd(detail, overwrite);
            if (added)
            {
                Version++;
            }

            return added;
        }

        public int AddDetails(IEnumerable<IDetail> details, bool overwrite = true)
        {
            var totalAdded = 0;
            foreach (var detail in details)
            {
                if (InternalAdd(detail, overwrite))
                    totalAdded++;
            }

            if (totalAdded > 0)
            {
                Version++;
            }
            return totalAdded;
        }

        public bool RemoveDetail(IDetail detail)
        {
            if (_details.Remove(detail.Name))
            {
                Version++;
                return true;
            }

            return false;
        }

        public IEnumerator<IDetail> GetEnumerator()
        {
            // ReSharper disable once HeapView.BoxingAllocation, details will not be value types.
            return _details.Values.GetEnumerator();
        }

        private bool InternalAdd(IDetail detail, bool overwrite)
        {
            if (!overwrite && _details.ContainsKey(detail.Name))
            {
                return false;
            }

            _details[detail.Name] = detail;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public bool Equals(IReference other)
        {
            return ReferenceEquals(this, other);
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var detail in _details)
            {
                sb.AppendLine($"{detail.Key}: {detail.Value.GetObject()}");
            }

            return sb.ToString();
        }
    }
}