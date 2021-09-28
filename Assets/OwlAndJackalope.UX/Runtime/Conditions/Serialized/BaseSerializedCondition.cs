using System;
using System.Reflection;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Conditions.Serialized
{
    [System.Serializable]
    public class BaseSerializedCondition : ISerializedCondition, IDetailNameChangeHandler
    {
        [SerializeField]
        private Parameter _parameterOne;
        
        [SerializeField]
        private Parameter _parameterTwo;

        [SerializeField]
        private BaseSerializedDetail _value;

        [SerializeField]
        private DetailType _type;

        [SerializeField]
        private int _enumId;

        [SerializeField]
        private Comparison _comparisonType;

        public static BaseSerializedCondition Create<T>(string parameterOne, string parameterTwo, Comparison comparison)
        {
            var condition = new BaseSerializedCondition();
            condition._type = typeof(T).ConvertToEnum();
            if (condition._type == DetailType.Enum)
            {
                condition._enumId = SerializedDetailEnumCache.GetCreator(typeof(T).Name).EnumId;
            }

            condition._parameterOne = new Parameter() { Type = ParameterType.Detail, Name = parameterOne };
            condition._parameterTwo = new Parameter() { Type = ParameterType.Detail, Name = parameterTwo };
            condition._comparisonType = comparison;
            return condition;
        }
        
        public static BaseSerializedCondition Create<T>(string parameterOne, T value, Comparison comparison)
        {
            var condition = new BaseSerializedCondition();
            condition._type = typeof(T).ConvertToEnum();
            if (condition._type == DetailType.Enum)
            {
                condition._enumId = SerializedDetailEnumCache.GetCreator(typeof(T).Name).EnumId;
            }

            condition._parameterOne = new Parameter() { Type = ParameterType.Detail, Name = parameterOne };
            condition._parameterTwo = new Parameter() { Type = ParameterType.Value };
            condition._value = new BaseSerializedDetail("", typeof(T));
            condition._value.SetValue(value);
            
            condition._comparisonType = comparison;
            return condition;
        }

        public ICondition ConvertToCondition()
        {
            switch (_type)
            {
                case DetailType.Bool:
                    return CreateComparableCondition<bool>();
                case DetailType.Integer:
                    return CreateComparableCondition<int>();
                case DetailType.Long:
                    return CreateComparableCondition<long>();
                case DetailType.Float:
                    return CreateComparableCondition<float>();
                case DetailType.Double:
                    return CreateComparableCondition<double>();
                case DetailType.Enum:
                    return CreateComparableEnumCondition();
                case DetailType.String:
                    return CreateComparableCondition<string>();
                case DetailType.TimeSpan:
                    return CreateComparableCondition<TimeSpan>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private ICondition CreateComparableCondition<T>() where T : IComparable<T>
        {
            if (_parameterTwo.Type == ParameterType.Value)
            {
                return new BaseRuntimeCondition<T>(_parameterOne, _value.ConvertToDetail() as IDetail<T>, _comparisonType);
            }
            return new BaseRuntimeCondition<T>(_parameterOne, _parameterTwo, _comparisonType);
        }

        private ICondition CreateComparableEnumCondition()
        {
            try
            {
                var creator = SerializedDetailEnumCache.GetCreator(_enumId);
                if (creator != null)
                {
                    return creator.CreateCondition(_comparisonType, _parameterOne, _parameterTwo, _value.ConvertToDetail());    
                }
                
                Debug.LogError($"{_enumId} does not have a defined enum creator");
                return null;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            if (_parameterOne.Name == previousName)
            {
                _parameterOne.Name = newName;
            }

            if (_parameterTwo.Name == previousName)
            {
                _parameterTwo.Name = newName;
            }
        }
    }
}