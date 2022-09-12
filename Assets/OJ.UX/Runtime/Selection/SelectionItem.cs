using System;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    public abstract class SelectionItem<TValue> : MonoBehaviour, IAlertable
    {
        private SelectionGroup<TValue> _selectionGroup;
        private ReferenceModule _module;

        protected virtual void Awake()
        {
            _selectionGroup = GetComponentInParent<SelectionGroup<TValue>>();
            _module = GetComponent<ReferenceModule>();
        }

        protected virtual void Start()
        {
            _selectionGroup.SelectedItems.OnChanged += HandleSelectionChanged;
        }

        protected virtual void OnDestroy()
        {
            _selectionGroup.SelectedItems.OnChanged -= HandleSelectionChanged;
        }

        public abstract TValue GetValue();

        public void AlertOfChange()
        {
            if(_selectionGroup != null && _selectionGroup.SelectedItems != null)
                HandleSelectionChanged();
        }

        public void Select()
        {
            if (_selectionGroup != null)
            {
                _selectionGroup.Select(GetValue());
            }
        }

        public void ToggleSelect()
        {
            if (_selectionGroup != null)
            {
                _selectionGroup.ToggleSelect(GetValue());
            }
        }

        public void Deselect()
        {
            if (_selectionGroup != null)
            {
                _selectionGroup.Deselect(GetValue());
            }
        }
        
        protected virtual void HandleSelectionChanged()
        {
            if (_module != null)
            {
                var isSelected = _selectionGroup.SelectedItems.Contains(GetValue());
                var detail = _module.Reference.GetDetail<bool>(SelectionConstants.IsSelected);
                if (detail is IMutableDetail<bool> mutableDetail)
                {
                    mutableDetail.Value = isSelected;
                }
            }
        }
    }
}