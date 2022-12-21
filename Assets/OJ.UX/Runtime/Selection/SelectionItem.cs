using System;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    public abstract class SelectionItem<TValue> : MonoBehaviour, IAlertable
    {
        private SelectionGroupBinder<TValue> _selectionGroupProvider;
        private ReferenceModule _module;

        protected virtual void Awake()
        {
            _selectionGroupProvider = GetComponentInParent<SelectionGroupBinder<TValue>>();
            _module = GetComponent<ReferenceModule>();
        }

        protected virtual void Start()
        {
            _selectionGroupProvider.OnSelectionChanged.AddListener(HandleSelectionChanged);
        }

        protected virtual void OnDestroy()
        {
            if (_selectionGroupProvider != null)
            {
                _selectionGroupProvider.OnSelectionChanged.RemoveListener(HandleSelectionChanged);
            }
        }

        public abstract TValue GetValue();

        public void AlertOfChange()
        {
            if(_selectionGroupProvider != null && _selectionGroupProvider.SelectedItems != null)
                HandleSelectionChanged();
        }

        public void Select()
        {
            if (_selectionGroupProvider != null)
            {
                _selectionGroupProvider.Select(GetValue());
            }
        }

        public void ToggleSelect()
        {
            if (_selectionGroupProvider != null)
            {
                _selectionGroupProvider.ToggleSelect(GetValue());
            }
        }

        public void Deselect()
        {
            if (_selectionGroupProvider != null)
            {
                _selectionGroupProvider.Deselect(GetValue());
            }
        }
        
        protected virtual void HandleSelectionChanged()
        {
            if (_module != null)
            {
                var isSelected = _selectionGroupProvider.SelectedItems.Contains(GetValue());
                var detail = _module.Reference.GetDetail<bool>(SelectionConstants.IsSelected);
                if (detail is IMutableDetail<bool> mutableDetail)
                {
                    mutableDetail.Value = isSelected;
                }
            }
        }
    }
}