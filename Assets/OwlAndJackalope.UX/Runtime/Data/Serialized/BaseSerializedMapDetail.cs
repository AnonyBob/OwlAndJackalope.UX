using System;
using System.Collections.Generic;
using System.Reflection;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    [System.Serializable]
    public class BaseSerializedMapDetail : ISerializedDetail
    {
        public string Name => _name;

        public Type Type => typeof(Dictionary<,>).MakeGenericType(
            _keyType.ConvertToType(_keyEnumId), _valueType.ConvertToType(_valueEnumId));
        
        [SerializeField] protected string _name;
        [SerializeField] protected DetailType _keyType;
        [SerializeField] protected int _keyEnumId;
        [SerializeField] protected DetailType _valueType;
        [SerializeField] protected int _valueEnumId;
        
        [SerializeField] private List<BaseSerializedDetail> _keyCollection = new List<BaseSerializedDetail>();
        [SerializeField] private List<BaseSerializedDetail> _valueCollection = new List<BaseSerializedDetail>();

        public BaseSerializedMapDetail(IMapDetail detail)
        {
            _name = detail.Name;
            var (keyType, valueType) = detail.GetItemType();
            _keyType = keyType.ConvertToEnum();
            _valueType = valueType.ConvertToEnum();
            
            if (_keyType == DetailType.Enum)
            {
                _keyEnumId = SerializedDetailEnumCache.GetCreator(keyType.Name).EnumId;
            }
            if (_valueType == DetailType.Enum)
            {
                _valueEnumId = SerializedDetailEnumCache.GetCreator(valueType.Name).EnumId;
            }
        }
        
        public IDetail ConvertToDetail()
        {
            var keyType = _keyType.ConvertToType(_keyEnumId);
            var valueType = _valueType.ConvertToType(_valueEnumId);
            
            var mapType = typeof(BaseMapDetail<,>).MakeGenericType(keyType, valueType);
            return (IDetail) Activator.CreateInstance(mapType, _name, ConstructDictionary(keyType, valueType), false);
        }

        private object ConstructDictionary(Type keyType, Type valueType)
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var addMethod = dictType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var dictionary = Activator.CreateInstance(dictType);
            for (var i = 0; i < _keyCollection.Count; ++i)
            {
                if (_valueCollection.Count <= i)
                {
                    break;
                }
                addMethod.Invoke(dictionary, new object[]
                {
                    _keyCollection[i].GetValue(keyType),
                    _valueCollection[i].GetValue(valueType)
                });
            }
            
            return dictionary;
        }
    }
}