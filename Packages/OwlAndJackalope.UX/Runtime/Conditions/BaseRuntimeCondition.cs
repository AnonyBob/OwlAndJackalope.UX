using System;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.Conditions
{
    public class BaseRuntimeCondition<TValue> : ICondition
    {
        private readonly Parameter _leftHandSide;
        private readonly Parameter _rightHandSide;
        private readonly IDetail<TValue> _valueParameter;
        private readonly Comparison _comparisonType;

        public BaseRuntimeCondition(Parameter leftHandSide, Parameter rightHandSide, Comparison comparisonType)
        {
            _leftHandSide = leftHandSide;
            _rightHandSide = rightHandSide;
            _valueParameter = null;
            _comparisonType = comparisonType;
        }

        public BaseRuntimeCondition(Parameter leftHandSide, IDetail<TValue> rightHandSide, Comparison comparisonType)
        {
            _leftHandSide = leftHandSide;
            _rightHandSide = new Parameter()
            {
                Type = ParameterType.Value
            };
            _valueParameter = rightHandSide;
            _comparisonType = comparisonType;
        }

        public IEnumerable<string> GetUsedDetails()
        {
            var leftHandName = GetDetailNameFromParameter(_leftHandSide);
            var rightHandName = GetDetailNameFromParameter(_rightHandSide);

            if (!string.IsNullOrEmpty(leftHandName))
            {
                yield return leftHandName;
            }

            if (!string.IsNullOrEmpty(rightHandName))
            {
                yield return rightHandName;
            }
        }

        public bool IsMet(IReference reference)
        {
            return IsMet(reference, null);
        }
        
        public bool IsMet(IReference reference, IDetail argument)
        {
            var argType = argument as IDetail<TValue>;
            var finalLeftHandSide = GetDetail(_leftHandSide, reference, argType);
            var finalRightHandSide = GetDetail(_rightHandSide, reference, argType);

            return CalculateComparison(finalLeftHandSide, finalRightHandSide);
        }

        private string GetDetailNameFromParameter(Parameter parameter)
        {
            if (parameter?.Type == ParameterType.Detail || parameter?.Type == ParameterType.DetailComponent)
            {
                return parameter.Name;
            }

            return null;
        }

        private IDetail<TValue> GetDetail(Parameter parameter, IReference reference, IDetail<TValue> argument)
        {
            switch (parameter.Type)
            {
                case ParameterType.Value:
                    return _valueParameter;
                case ParameterType.Detail:
                    return reference.GetDetail<TValue>(parameter.Name);
                case ParameterType.Argument:
                    return argument;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private bool CalculateComparison(IDetail<TValue> one, IDetail<TValue> two)
        {
            var compare = Comparer<TValue>.Default.Compare(one.GetValue(), two.GetValue());
            switch (_comparisonType)
            {
                case Comparison.Equal:
                    return compare == 0;
                case Comparison.NotEqual:
                    return compare != 0;
                case Comparison.GreaterThan:
                    return compare > 0;
                case Comparison.GreaterThanEqual:
                    return compare >= 0;
                case Comparison.LessThan:
                    return compare < 0;
                case Comparison.LessThanEqual:
                    return compare <= 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}