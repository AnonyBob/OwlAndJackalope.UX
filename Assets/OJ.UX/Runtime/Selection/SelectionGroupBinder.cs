using System;
using OJ.UX.Runtime.Binders;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;
using UnityEngine.Events;

namespace OJ.UX.Runtime.Selection
{
    public class SelectionGroupBinder<TValue> : AbstractDetailBinder
    {
        public IDetail<TValue> SelectedItem => _selectedItem.Detail;
        
        public ListDetail<TValue> SelectedItems => _selectedList.ListDetail;

        [SerializeField, Tooltip("When true, if there is a max selection count set, when exceeded the oldest item will be deselected.")] 
        private bool _deselectOldest = false;
        
        [SerializeField]
        private ListObserver<TValue> _selectedList;
        
        [SerializeField]
        private Observer<TValue> _selectedItem;
        
        [SerializeField]
        private Observer<int> _selectedCount;
        
        [SerializeField]
        private Observer<int> _maxCount;
        
        [SerializeField]
        private Observer<bool> _atMaxSelected;
        
        [SerializeField]
        public UnityEvent OnSelectionChanged;

        private void Start()
        {
            _selectedList.Initialize(HandleSelectionChanged);
            _selectedItem.Initialize();
            _selectedCount.Initialize();
            _maxCount.Initialize();
            _atMaxSelected.Initialize();
        }

        public bool Select(TValue item)
        {
            if (_selectedList.Contains(item))
            {
                return false;
            }
            
            if (_atMaxSelected.Value)
            {
                if (_deselectOldest)
                {
                    Deselect(_selectedList[0]);
                }
                else
                {
                    return false;    
                }
            }
            
            _selectedList.Add(item);
            return true;
        }

        public void ToggleSelect(TValue item)
        {
            if (_selectedList.Contains(item))
            {
                Deselect(item);
            }
            else
            {
                Select(item);
            }
        }

        public void SelectMultiple(params TValue[] items)
        {
            foreach (var item in items)
            {
                Select(item);
            }
        }

        public bool Deselect(TValue item)
        {
            var deselected = _selectedList.Remove(item);
            return deselected;
        }

        public void DeselectAll()
        {
            for (var i = _selectedCount.Value - 1; i >= 0; --i)
            {
                Deselect(_selectedList[i]);
            }
        }
        
        private void HandleSelectionChanged()
        {
            OnSelectionChanged?.Invoke();
        }

        private void OnDestroy()
        {
            _selectedList.Destroy();
            _selectedItem.Destroy();
            _selectedCount.Destroy();
            _maxCount.Destroy();
            _atMaxSelected.Destroy();
        }
    }
}