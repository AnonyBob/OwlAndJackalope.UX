using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable, ConditionDisplay("Integer Comparison", "Integer")]
    public class ConditionIntComparison : ConditionValueComparison<int>
    {
        
    }
    
    [System.Serializable, ConditionDisplay("Long Comparison", "Long")]
    public class ConditionLongComparison : ConditionValueComparison<long>
    {
        
    }
    
    [System.Serializable, ConditionDisplay("Double Comparison", "Double")]
    public class ConditionDoubleComparison : ConditionValueComparison<double>
    {
        
    }
    
    [System.Serializable, ConditionDisplay("Float Comparison", "Float")]
    public class ConditionFloatComparison : ConditionValueComparison<float>
    {
        
    }
    
    [System.Serializable, ConditionDisplay("Bool Comparison", "Bool")]
    public class ConditionBoolComparison : ConditionValueComparison<bool>
    {
        
    }
    
    [System.Serializable, ConditionDisplay("String Comparison", "String")]
    public class ConditionStringComparison : ConditionValueComparison<string>
    {
    }

    [System.Serializable, ConditionDisplay("Enum Comparison", "Enum")]
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