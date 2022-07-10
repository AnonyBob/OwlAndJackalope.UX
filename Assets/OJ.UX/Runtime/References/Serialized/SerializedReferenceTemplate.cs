using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable, CreateAssetMenu(menuName = "OJ.UX/Reference Template", fileName = "Reference Template")]
    public class SerializedReferenceTemplate : ScriptableObject, ISerializedReference
    {
        [SerializeReference]
        public List<ISerializedDetail> Details;

        public IMutableReference CreateReference()
        {
            return new Reference(Details.Select(d => new KeyValuePair<string, IDetail>(d.GetName(), d.CreateDetail())));
        }
    }
}