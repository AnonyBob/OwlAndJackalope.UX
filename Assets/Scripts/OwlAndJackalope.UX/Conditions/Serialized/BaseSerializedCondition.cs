using System;
using System.Diagnostics;
using OwlAndJackalope.UX.Data;
using OwlAndJackalope.UX.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Conditions.Serialized
{
    [System.Serializable]
    public class BaseSerializedCondition : ISerializedCondition
    {
        [SerializeField]
        private Parameter _parameterOne;
        
        [SerializeField]
        private Parameter _parameterTwo;

        [SerializeField]
        private BaseSerializedDetail _value;

        [SerializeField]
        private DetailType _detailType;
        
        [SerializeField]
        private Comparison _comparisonType;
        
        public ICondition ConvertToCondition()
        {
            switch (_detailType)
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
                    break;
                case DetailType.String:
                    return CreateComparableCondition<string>();
                case DetailType.Reference:
                    break;
                case DetailType.Vector2:
                    break;
                case DetailType.Vector3:
                    break;
                case DetailType.Color:
                    break;
                case DetailType.Custom:
                    return CreateCustom();
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
        
        protected virtual ICondition CreateCustom()
        {
            throw new NotImplementedException();
        }
    }
}