using System;
using System.Collections.Generic;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Extensions;
using OwlAndJackalope.UX.Data.Serialized;
using OwlAndJackalope.UX.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OwlAndJackalope.UX.Testing
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