using OJ.UX.Runtime.Binders.ListBinders;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    [RequireComponent(typeof(ReferenceInitializationObject))]
    public class SelectableItemReference : SelectableItem
    {
        private ReferenceInitializationObject _target;

        protected override void Awake()
        {
            base.Awake();
            _target = GetComponent<ReferenceInitializationObject>();
        }

        protected override object InternalGetItem()
        {
            return _target.Reference;
        }
    }
}