using System.Collections.Generic;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binding
{
    public abstract class DetailsProvider : MonoBehaviour
    {
        public abstract IEnumerable<KeyValuePair<string, IDetail>> ProvideDetails();
    }
}