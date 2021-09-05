using System.Collections;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Testing
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