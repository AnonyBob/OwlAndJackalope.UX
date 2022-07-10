using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class ConditionValueComparison<TValue> : IConditionComparison 
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