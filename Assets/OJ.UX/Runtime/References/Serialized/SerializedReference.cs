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

        public IReference CreateReference()
        {
            return new Reference(Details.Select(d => new KeyValuePair<string, IDetail>(d.GetName(), d.CreateDetail())));
        }
    }
}