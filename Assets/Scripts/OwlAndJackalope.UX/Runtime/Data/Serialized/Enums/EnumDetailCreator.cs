using System;
using System.Collections.Generic;
using System.Linq;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized.Enums
{
    public interface IEnumDetailCreator
    {
        string EnumName { get; }
        Type EnumType { get; }

        object Convert(int value);
        
        IDetail CreateDetail(string name, int value);
        IDetail CreateCollectionDetail(string name, IEnumerable<int> value);
        IDetail CreateMapDetailWithKey<TKey>(string name, IEnumerable<(TKey key, int value)> entries);
        IDetail CreateMapDetailWithValue<TValue>(string name, IEnumerable<(int key, TValue value)> entries);
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
        
        public IDetail CreateMapDetailWithKey<TKey>(string name, IEnumerable<(TKey key, int value)> entries)
        {
            return new BaseMapDetail<TKey, TEnumType>(name, entries.ToDictionary(t => t.key, 
                t => Converter(t.value)), false);
        }
        
        public IDetail CreateMapDetailWithValue<TValue>(string name, IEnumerable<(int key, TValue value)> entries)
        {
            return new BaseMapDetail<TEnumType, TValue>(name, entries.ToDictionary(t => Converter(t.key), 
                t => t.value), false);
        }
    }
}