using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class AndConditionGroup : ICondition, IConditionChangedHandler, IDetailBinder
    {
        [SerializeField]
        private Condition[] _conditions;
        private IConditionChangedHandler _handler;

        public void Initialize(IConditionChangedHandler handler)
        {
            _handler = handler;
            if (_conditions != null)
            {
                foreach (var condition in _conditions)
                    condition.Initialize(this);
            }
        }

        public bool IsConditionMet()
        {
            if (_conditions != null)
            {
                foreach (var condition in _conditions)
                {
                    if (!condition.IsConditionMet())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public void ConditionHasChanged(ICondition condition)
        {
            _handler.ConditionHasChanged(this);
        }

        public void Destroy()
        {
            if (_conditions != null)
            {
                foreach (var condition in _conditions)
                    condition.Destroy();
            }
        }

        public bool RespondToNameChange(ReferenceModule changingModule, string originalName, string newName)
        {
            if (_conditions == null)
                return false;

            var didChange = false;
            foreach (var condition in _conditions)
            {
                didChange = condition.RespondToNameChange(changingModule, originalName, newName) || didChange;
            }

            return didChange;
        }
    }
}