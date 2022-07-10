using System;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class AndConditionGroup : ICondition, IConditionChangedHandler
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

        public void Dispose()
        {
            if (_conditions != null)
            {
                foreach (var condition in _conditions)
                    condition.Dispose();
            }
        }
    }
}