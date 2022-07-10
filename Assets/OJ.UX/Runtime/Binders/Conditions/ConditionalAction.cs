using System;
using System.Linq;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class ConditionalAction<TActionDetail> : IConditionChangedHandler, IDisposable
        where TActionDetail : IConditionalActionDetail
    {
        [SerializeField]
        private string _actionDescription;

        [SerializeField]
        private AndConditionGroup[] _conditionGroups;

        [SerializeField]
        private TActionDetail _action;

        private bool? _previousConditionStatus;
        
        public void Initialize()
        {
            if (_conditionGroups != null)
            {
                foreach (var group in _conditionGroups)
                    group.Initialize(this);
            }

            CheckConditionStatusAndPerformAction();
        }
        
        public void ConditionHasChanged(ICondition condition)
        {
            CheckConditionStatusAndPerformAction();
        }

        private void CheckConditionStatusAndPerformAction()
        {
            var conditionIsMet = _conditionGroups.Any(g => g.IsConditionMet());
            if (_previousConditionStatus.HasValue)
            {
                if(conditionIsMet && !_previousConditionStatus.Value)
                    _action.PerformAction();
            }
            else if (conditionIsMet) //If this is the first time we are performing the action.
            {
                _action.PerformAction();
            }

            _previousConditionStatus = conditionIsMet;
        }

        public void Dispose()
        {
            if (_conditionGroups != null)
            {
                foreach (var group in _conditionGroups)
                    group.Dispose();
            }
        }
    }
}