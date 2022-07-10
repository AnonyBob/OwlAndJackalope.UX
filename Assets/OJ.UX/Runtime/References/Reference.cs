using System;
using System.Collections;
using System.Collections.Generic;

namespace OJ.UX.Runtime.References
{
    public class Reference : IMutableReference
    {
        public event Action OnChanged;
        
        public long Version { get; private set; }

        private readonly Dictionary<string, IDetail> _details = new Dictionary<string, IDetail>();

        public Reference()
        {
            
        }
        
        public Reference(IEnumerable<KeyValuePair<string, IDetail>> details)
        {
            if (details != null)
            {
                foreach (var detail in details)
                {
                    _details[detail.Key] = detail.Value;
                }   
            }
        }

        public Reference(params (string DetailName, IDetail Detail)[] details)
        {
            if (details != null)
            {
                foreach (var detail in details)
                {
                    _details[detail.DetailName] = detail.Detail;
                }   
            }
        }

        public IDetail GetDetail(string detailName)
        {
            if (detailName == null)
                throw new ArgumentNullException(nameof(detailName));
            
            _details.TryGetValue(detailName, out var detail);
            return detail;
        }

        public IDetail<TValue> GetDetail<TValue>(string detailName)
        {
            var detail = GetDetail(detailName);
            return detail as IDetail<TValue>;
        }

        public bool AddDetail(string detailName, IDetail detail, bool allowOverwrite = true)
        {
            if (allowOverwrite || !_details.ContainsKey(detailName))
            {
                _details[detailName] = detail;
                Version++;
                
                OnChanged?.Invoke();
                return true;
            }

            return false;
        }

        public int AddDetails(IEnumerable<KeyValuePair<string, IDetail>> details, bool allowOverwrite = true)
        {
            var detailsAdded = 0;
            if (details != null)
            {
                foreach (var detail in details)
                {
                    if (allowOverwrite || !_details.ContainsKey(detail.Key))
                    {
                        _details[detail.Key] = detail.Value;
                        detailsAdded ++;
                    }
                }
            }

            if (detailsAdded > 0)
            {
                Version++;
                OnChanged?.Invoke();
            }

            return detailsAdded;
        }

        public bool RemoveDetail(string detailName)
        {
            var removed = _details.Remove(detailName);
            if (removed)
            {
                Version++;
                OnChanged?.Invoke();
            }

            return removed;
        }
        
        public IEnumerator<KeyValuePair<string, IDetail>> GetEnumerator()
        {
            return _details.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}