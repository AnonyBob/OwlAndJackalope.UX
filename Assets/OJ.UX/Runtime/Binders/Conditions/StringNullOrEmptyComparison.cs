using System;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    public enum StringCompare
    {
        IsNullOrEmpty,
        IsNotNullOrEmpty
    }
    
    [Serializable, ConditionDisplay("String is Null or Empty", "String")]
    public class StringNullOrEmptyComparison : IConditionComparison
    {
        [SerializeField]
        private StringCompare _compare; 
        
        public Type GetConditionValueType()
        {
            return typeof(string);
        }

        public bool CheckCondition(Observer observer)
        {
            var value = observer.GetValue<string>();
            if (_compare == StringCompare.IsNullOrEmpty)
                return string.IsNullOrEmpty(value);
            else
                return !string.IsNullOrEmpty(value);
        }
    }
}