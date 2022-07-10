using System;
using OJ.UX.Runtime.References.Serialized;
using UnityEngine;

namespace OJ.UX.Runtime.References
{
    public class Test : MonoBehaviour
    {
        public SerializedReference reference;
        
        [ContextMenu("Add Int Detail")]
        public void AddDetail()
        {
            var detail = new SerializedIntDetail();
            detail.Name = Guid.NewGuid().ToString().Substring(0, 5);
            reference.Details.Add(detail);
        }
    }
}