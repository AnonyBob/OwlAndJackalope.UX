using System.Collections.Generic;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    public class SelectionGroup<TValue> : DetailsProvider
    {
        [SerializeField, Tooltip("Amount that can be selected at one time. Set to zero to have unlimited.")]
        private int _maxSelectionCount = 1;

        [SerializeField, Tooltip("When true, if there is a max selection count set, when exceeded the oldest item will be deselected.")] 
        private bool _deselectOldest = false;
        
        private IReference _reference;
        private ListDetail<TValue> _selectedList;
        private IDetail<TValue> _selectedItem;
        private IDetail<int> _selectedCount;
        private IDetail<int> _maxSelectionCountDetail;
        private IDetail<bool> _atMaxSelected;

        public IDetail<TValue> SelectedItem => _selectedItem;
        public ListDetail<TValue> SelectedItems => _selectedList;

        private void Awake()
        {
            if (_reference == null)
            {
                CreateReference();
            }
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

        private void CreateReference()
        {
            _selectedList = new ListDetail<TValue>();
            _selectedItem = new TransformDetail<List<TValue>, TValue>(list =>
            {
                if (list == null || list.Count == 0)
                {
                    return default(TValue);
                }

                return list[0];
            }, _selectedList);
            
            _maxSelectionCountDetail = new Detail<int>(_maxSelectionCount);
            _selectedCount = new TransformDetail<List<TValue>, int>(list => list?.Count ?? 0, _selectedList);
            _atMaxSelected = new TransformDetail<int, int, bool>(
                (selectedCount, maxSelectedCount) => selectedCount >= maxSelectedCount && maxSelectedCount != 0, 
                _selectedCount, _maxSelectionCountDetail);
            
            
            _reference = new Reference(
                (SelectionConstants.SelectedItem, _selectedItem),
                (SelectionConstants.SelectedList, _selectedList),
                (SelectionConstants.SelectedCount, _selectedCount),
                (SelectionConstants.AtMaxSelected, _atMaxSelected),
                (SelectionConstants.MaxAllowed, _maxSelectionCountDetail));
        }
        
        public override IEnumerable<KeyValuePair<string, IDetail>> ProvideDetails()
        {
            if (_reference == null)
            {
                CreateReference();
            }

            return _reference;
        }
    }
}