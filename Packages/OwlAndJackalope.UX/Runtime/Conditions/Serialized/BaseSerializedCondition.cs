using System;
using System.Reflection;
using OwlAndJackalope.UX.Runtime.Data;
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
                case DetailType.Reference:
                    break;
                case DetailType.Vector2:
                    break;
                case DetailType.Vector3:
                    break;
                case DetailType.Color:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
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
                var conditionType = typeof(BaseRuntimeCondition<>).MakeGenericType(creator?.EnumType);
                if (_parameterTwo.Type == ParameterType.Value)
                {
                    return (ICondition)Activator.CreateInstance(conditionType, _parameterOne, _value.ConvertToDetail(), _comparisonType);
                }
                return (ICondition)Activator.CreateInstance(conditionType, _parameterOne, _parameterTwo, _comparisonType);
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