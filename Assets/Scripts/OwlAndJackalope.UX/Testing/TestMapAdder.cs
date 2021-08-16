using System.Collections.Generic;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Extensions;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Testing
{
    public class TestMapAdder : MonoBehaviour
    {
        public string DetailName;

        private IMutableMapDetail<long, Color> _detail;
        
        private void Start()
        {
            _detail = GetComponentInParent<ReferenceModule>().Reference.GetMutableMap<long, Color>(DetailName);
        }
        
        
        [ContextMenu("Add Item")]
        private void AddItem()
        {
            var number = Random.Range(0, 30000);
            while (_detail.ContainsKey(number))
            {
                number++;
            }
            _detail.Add(number, new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1));
        }

        [ContextMenu("Set Value")]
        private void SetValue()
        {
            _detail.SetValue(new Dictionary<long, Color>()
            {
                {1, Color.black},
                {2, Color.red},
                {3, Color.blue},
                {4, Color.green},
                {5, Color.yellow},
            });
        }
    }
}