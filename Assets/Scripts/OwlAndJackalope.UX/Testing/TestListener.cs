using System;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Testing
{
    public class TestListener : MonoBehaviour
    {
        public string DetailName;
        public GameObject TargetObject;
        private IDetail<bool> _detail;
        
        public void Start()
        {
            _detail = GetComponentInParent<ReferenceModule>().Reference.GetDetail<bool>(DetailName);
            _detail.VersionChanged += HandleChange;
            HandleChange();
        }

        private void OnDestroy()
        {
            if (_detail != null)
            {
                _detail.VersionChanged -= HandleChange;
            }
        }

        private void HandleChange()
        {
            TargetObject.SetActive(_detail.GetValue());
        }
    }
}