using OJ.UX.Runtime.References;

namespace OJ.UX.Runtime.Selection
{
    public class ReferenceSelectionGroupReference : SelectionGroupReference<IReference>
    {
        protected override void OnSelectionChanged(IReference item, bool selected)
        {
            var selectedDetail = item.GetDetail<bool>(SelectionConstants.IsSelected);
            if (selectedDetail is IMutableDetail<bool> mutableSelectedDetail)
            {
                mutableSelectedDetail.Value = selected;
            }
        }
    }
}