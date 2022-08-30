using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public abstract class ConditionConversionValueComparison<TStoredValue, TFinalValue> : IConditionValueComparison<TFinalValue> 
        where TFinalValue : IComparable<TFinalValue>
    {
        [SerializeField]
        protected TStoredValue _comparisonValue;
        
        [SerializeField]
        protected ComparisonType _comparisonType;
        
        public Type GetConditionValueType()
        {
            return typeof(TFinalValue);
        }

        public bool CheckCondition(Observer observer)
        {
            var value = ConvertToFinalValue(_comparisonValue);
            var observedValue = observer.GetValue<TFinalValue>();

            return observedValue.DoComparison(_comparisonType, value);
        }

        protected abstract TFinalValue ConvertToFinalValue(TStoredValue value);
        
        protected abstract TStoredValue ConvertToStoredValue(TFinalValue value);
    }
}