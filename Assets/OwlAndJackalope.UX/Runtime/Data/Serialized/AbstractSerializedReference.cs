using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    [System.Serializable]
    public abstract class AbstractSerializedReference<TDetail, TCollectionDetail, TMapDetail> : ISerializedReference 
        where TDetail : ISerializedDetail
        where TCollectionDetail : ISerializedDetail
        where TMapDetail : ISerializedDetail
    {
        [SerializeField] protected List<TDetail> _details = new List<TDetail>();
        [SerializeField] protected List<TCollectionDetail> _collectionDetails = new List<TCollectionDetail>();
        [SerializeField] protected List<TMapDetail> _mapDetails = new List<TMapDetail>();

        public virtual IReference ConvertToReference()
        {
            return new BaseReference(_details
                .Select(x => x.ConvertToDetail())
                .Union(_collectionDetails.Select(d => d.ConvertToDetail()))
                .Union(_mapDetails.Select(d => d.ConvertToDetail()))
                .Where(x => x != null));
        }

        public void UpdateSerializedDetails(IEnumerable<IDetail> reference)
        {
            foreach (var detail in reference)
            {
                if (detail is ICollectionDetail collectionDetail)
                {
                    var contained = _collectionDetails.FirstOrDefault(d => d.Name == detail.Name);
                    if (contained == null)
                    {
                        _collectionDetails.Add(CreateSerializedCollectionDetail(collectionDetail));
                    }
                }
                else if (detail is IMapDetail mapDetail)
                {
                    var contained = _mapDetails.FirstOrDefault(d => d.Name == detail.Name);
                    if (contained == null)
                    {
                        _mapDetails.Add(CreateSerializedMapDetail(mapDetail));
                    }
                }
                else
                {
                    var contained = _details.FirstOrDefault(d => d.Name == detail.Name);
                    if (contained == null)
                    {
                        _details.Add(CreateSerializedDetail(detail));
                    }
                }
            }
        }

        public IEnumerable<ISerializedDetail> GetDetails(Type[] acceptableTypes)
        {
            return _details
                .Where(x => acceptableTypes.Any(t => t.IsAssignableFrom(x.Type))).Cast<ISerializedDetail>()
                .Union(_collectionDetails
                    .Where(x => acceptableTypes.Contains(x.Type)).Cast<ISerializedDetail>())
                .Union(_mapDetails
                    .Where(x => acceptableTypes.Contains(x.Type)).Cast<ISerializedDetail>());
        }

        protected abstract TDetail CreateSerializedDetail(IDetail detail);
        protected abstract TCollectionDetail CreateSerializedCollectionDetail(ICollectionDetail detail);
        protected abstract TMapDetail CreateSerializedMapDetail(IMapDetail detail);
        
        
        public IEnumerator<ISerializedDetail> GetEnumerator()
        {
            return _details.Cast<ISerializedDetail>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}