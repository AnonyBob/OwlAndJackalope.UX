using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable]
    public class SerializedReference : ISerializedReference
    {
        [SerializeReference]
        public List<ISerializedDetail> Details;

        public IMutableReference CreateReference()
        {
            return new Reference(Details.Select(d => new KeyValuePair<string, IDetail>(d.GetName(), d.CreateDetail())));
        }

        public void LinkDetail(string detailName, IDetail detail)
        {
            var linkedDetail = Details.FirstOrDefault(d => d.GetName() == detailName);
            if (linkedDetail == null)
            {
                linkedDetail = ConstructFromDetail(detail);
                Details.Add(linkedDetail);
            }
            
            linkedDetail.LinkRuntimeDetail(detail, true);
        }

        private ISerializedDetail ConstructFromDetail(IDetail detail)
        {
            return null;
        }
        
        public IEnumerable<ISerializedDetail> CopyDetails()
        {
            if (Details == null)
            {
                yield break;
            }
            
            foreach (var detail in Details)
            {
                if (detail != null)
                {
                    yield return detail.Copy();
                }
            }
        }

        public void AddDetails(IEnumerable<ISerializedDetail> details)
        {
            if (details == null)
            {
                return;
            }

            if (Details == null)
            {
                Details = new List<ISerializedDetail>();
            }

            foreach (var detail in details)
            {
                if (Details.FirstOrDefault(x => x.GetName() == detail.GetName()) == null)
                {
                    Details.Add(detail);
                }
            }
        }
    }
}