using System.Collections.Generic;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Selection
{
    public abstract class SelectionGroupProvider<TValue> : DetailsProvider
    {
        [SerializeField, Tooltip("Amount that can be selected at one time. Set to zero to have unlimited.")]
        private int _maxSelectionCount = 1;
        
        private IReference _reference;
        private ListDetail<TValue> _selectedList;
        private IDetail<TValue> _selectedItem;
        private IDetail<int> _selectedCount;
        private IDetail<int> _maxSelectionCountDetail;
        private IDetail<bool> _atMaxSelected;
        
        private void Awake()
        {
            if (_reference == null)
            {
                CreateReference();
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