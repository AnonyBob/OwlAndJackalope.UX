using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Conditions;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized.Enums
{
    public interface IEnumDetailCreator
    {
        string EnumName { get; }
        Type EnumType { get; }

        object Convert(int value);
        
        IDetail CreateDetail(string name, int value);
        IDetail CreateCollectionDetail(string name, IEnumerable<int> value);
        ICondition CreateCondition(Comparison comparison, Parameter parameterOne, Parameter parameterTwo, IDetail comparisonValue);
    }
    
    public class EnumDetailCreator<TEnumType> : IEnumDetailCreator
    {
        public Func<int, TEnumType> Converter { get; }

        public string EnumName => EnumType.Name;
        public Type EnumType => typeof(TEnumType);
        
        public EnumDetailCreator(Func<int, TEnumType> converter)
        {
            Converter = converter;
        }

        public object Convert(int value)
        {
            return Converter(value);
        }
        
        public IDetail CreateDetail(string name, int value)
        {
            return new BaseDetail<TEnumType>(name, Converter(value));
        }

        public IDetail CreateCollectionDetail(string name, IEnumerable<int> values)
        {
            return new BaseCollectionDetail<TEnumType>(name, values.Select(Converter), false);
        }

        public ICondition CreateCondition(Comparison comparison, Parameter parameterOne, Parameter parameterTwo, IDetail comparisonValue)
        {
            if (parameterTwo.Type == ParameterType.Value)
            {
                return new BaseRuntimeCondition<TEnumType>(parameterOne, parameterTwo, comparison);
            }

            return new BaseRuntimeCondition<TEnumType>(parameterOne, comparisonValue as IDetail<TEnumType>, comparison);
        }
    }
}