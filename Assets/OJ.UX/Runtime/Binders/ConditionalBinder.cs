using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    [System.Serializable]
    public abstract class ConditionalBinder<TActionDetail> : AbstractDetailBinder
        where TActionDetail : IConditionalActionDetail
    {
        [SerializeField]
        private ConditionalAction<TActionDetail>[] _conditionalActions;

        public override bool RespondToNameChange(ReferenceModule changingModule, string originalName, string newName)
        {
            if (_conditionalActions == null)
                return false;

            var didChange = false;
            foreach (var conditionalAction in _conditionalActions)
            {
                didChange = conditionalAction.RespondToNameChange(changingModule, originalName, newName) || didChange;
            }

            if (didChange)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            return didChange;
        }
    }
}