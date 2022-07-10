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
            if(linkedDetail != null)
                linkedDetail.LinkRuntimeDetail(detail, true);
        }
    }
}