using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    public class SelectableItem : MonoBehaviour
    {
        [SerializeField]
        private SelectionGroupDetailsProvider _group;
        protected ReferenceModule _module;

        protected virtual void Awake()
        {
            _group = GetComponentInParent<SelectionGroupDetailsProvider>();
            _module = GetComponent<ReferenceModule>();
        }
        
        public void Select()
        {
            if (_group != null)
            {
                _group.Select(gameObject);
            }
        }

        public void ToggleSelect()
        {
            if (_group != null)
            {
                _group.ToggleSelect(gameObject);
            }
        }

        public void Deselect()
        {
            if (_group != null)
            {
                _group.Deselect(gameObject);
            }
        }

        public object GetItem()
        {
            return InternalGetItem();
        }
        
        public void SetSelectionStatus(bool selected)
        {
            InternalSetSelectionStatus(selected);
        }

        protected virtual object InternalGetItem()
        {
            return gameObject;
        }

        protected virtual void InternalSetSelectionStatus(bool selected)
        {
            if (_module != null)
            {
                var detail = _module.Reference.GetDetail<bool>(SelectionConstants.IsSelected);
                if (detail is IMutableDetail<bool> mutableDetail)
                {
                    mutableDetail.Value = selected;
                }
            }
        }
    }
}