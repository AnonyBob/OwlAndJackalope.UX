using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Testing
{
    public class TestAdder : MonoBehaviour
    {
        public string DetailName;

        public IMutableCollectionDetail<DetailType> _detail;

        private void Start()
        {
            _detail = GetComponentInParent<ReferenceModule>().Reference.GetMutableCollection<DetailType>(DetailName);
        }

        [ContextMenu("Add Item")]
        private void AddItem()
        {
            _detail.Add(DetailType.Double);
        }

        [ContextMenu("Set Value")]
        private void SetValue()
        {
            _detail.SetValue(new List<DetailType>()
            {
                DetailType.Bool,
                DetailType.Color,
                DetailType.Float
            });
        }
    }
}