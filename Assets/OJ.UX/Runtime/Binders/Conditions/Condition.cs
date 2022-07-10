using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class Condition : ICondition
    {
        [SerializeField]
        private Observer _observer;

        [SerializeField, SerializeReference]
        private IConditionComparison _comparison;

        private IConditionChangedHandler _handler;
        
        public void Initialize(IConditionChangedHandler handler)
        {
            _handler = handler;
            _observer.Initialize(AlertConditionHandler, true);
        }
        
        public bool IsConditionMet()
        {
            return _comparison.CheckCondition(_observer);
        }

        private void AlertConditionHandler()
        {
            _handler.ConditionHasChanged(this);
        }
        
        public void Dispose()
        {
            _observer.Dispose();
        }
    }
}