using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public interface IConditionValueComparison<TValue> : IConditionComparison 
        where TValue : IComparable<TValue>
    {
        
    }
    
    [System.Serializable]
    public abstract class ConditionValueComparison<TValue> : IConditionValueComparison<TValue>
        where TValue : IComparable<TValue>
    {
        [SerializeField]
        private TValue _comparisonValue;

        [SerializeField]
        private ComparisonType _comparisonType;

        public Type GetConditionValueType()
        {
            return typeof(TValue);
        }
        
        public bool CheckCondition(Observer observer)
        {
            return observer.GetValue<TValue>().DoComparison(_comparisonType, _comparisonValue);
        }
    }
}