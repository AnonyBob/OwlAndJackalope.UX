using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class ConditionIntComparison : ConditionValueComparison<int>
    {
        
    }
    
    [System.Serializable]
    public class ConditionLongComparison : ConditionValueComparison<long>
    {
        
    }
    
    [System.Serializable]
    public class ConditionDoubleComparison : ConditionValueComparison<double>
    {
        
    }
    
    [System.Serializable]
    public class ConditionFloatComparison : ConditionValueComparison<float>
    {
        
    }
    
    [System.Serializable]
    public class ConditionBoolComparison : ConditionValueComparison<bool>
    {
        
    }

    [System.Serializable]
    public class ConditionEnumComparison : IConditionComparison
    {
        [SerializeField]
        private int _comparisonValue;

        [SerializeField]
        private ComparisonType _comparisonType;
        
        public Type GetConditionValueType()
        {
            return typeof(Enum);
        }

        public bool CheckCondition(Observer observer)
        {
            var enumValue = (int)observer.ObjectDetail.Value;
            return enumValue.DoComparison(_comparisonType, _comparisonValue);
        }
    }
}