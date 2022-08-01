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

        private void Start()
        {
            BeforeConditionInitialized();
            if (_conditionalActions != null)
            {
                for (var i = 0; i < _conditionalActions.Length; ++i)
                {
                    _conditionalActions[i].Initialize();
                }
            }
            
            AfterConditionInitialized();
        }

        protected virtual void BeforeConditionInitialized()
        {
            
        }
        
        protected virtual void AfterConditionInitialized()
        {
            
        }

        private void OnDestroy()
        {
            BeforeConditionsCleared();
            if (_conditionalActions != null)
            {
                for (var i = 0; i < _conditionalActions.Length; ++i)
                {
                    _conditionalActions[i].Clear();
                }
            }
            
            AfterConditionsCleared();
        }

        protected virtual void BeforeConditionsCleared()
        {
            
        }
        
        protected virtual void AfterConditionsCleared()
        {
            
        }

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