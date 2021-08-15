using System;
using System.Collections;
using OwlAndJackalope.UX.Data.Extensions;
using OwlAndJackalope.UX.Data.Serialized;
using OwlAndJackalope.UX.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Testing
{
    public class TestUpdater : MonoBehaviour
    {
        public string DetailName;
        public DetailType DetailType;
        
        private IEnumerator Start()
        {
            var detail = GetComponentInParent<ReferenceModule>().Reference.GetMutable<DetailType>(DetailName);
            while (true)
            {
                yield return new WaitForSeconds(.4f);
                detail.SetValue(DetailType);
            }
        }
    }
}