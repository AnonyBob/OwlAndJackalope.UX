using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized
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

        public IEnumerable<ISerializedDetail> GetDetails(Type[] acceptableTypes)
        {
            return _details
                .Where(x => acceptableTypes.Contains(x.Type)).Cast<ISerializedDetail>()
                .Union(_collectionDetails
                    .Where(x => acceptableTypes.Contains(x.Type)).Cast<ISerializedDetail>())
                .Union(_mapDetails
                    .Where(x => acceptableTypes.Contains(x.Type)).Cast<ISerializedDetail>());
        }

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