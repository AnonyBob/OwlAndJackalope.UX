using System;
using System.Collections.Generic;
using System.Reflection;
using OwlAndJackalope.UX.Data.Extensions;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Serialized
{
    /// <summary>
    /// Serializes a collection detail by simply serializing a collection of serialized details.
    /// </summary>
    [System.Serializable]
    public class BaseSerializedCollectionDetail : ISerializedDetail
    {
        [SerializeField] protected string _name;
        [SerializeField] protected DetailType _type;
        [SerializeField] protected string _enumTypeName = "";
        [SerializeField] protected string _enumAssemblyName = "";
        
        [SerializeField] private List<BaseSerializedDetail> _collection = new List<BaseSerializedDetail>();
        public IDetail ConvertToDetail()
        {
            if (_type == DetailType.Custom)
            {
                return CreateCustomDetail();
            }

            var type = _type.ConvertToType(_enumTypeName, _enumAssemblyName);
            var collectionType = typeof(BaseCollectionDetail<>).MakeGenericType(type);
            return (IDetail) Activator.CreateInstance(collectionType, _name, ConstructList(type), false);
        }

        private object ConstructList(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            var addMethod = listType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var list = Activator.CreateInstance(listType);
            for (var i = 0; i < _collection.Count; ++i)
            {
                addMethod.Invoke(list, new object[] {_collection[i].GetValue(type)});
            }

            return list;
        }

        protected virtual IDetail CreateCustomDetail()
        {
            return null; //OVERRIDE TO ADD EVEN MORE!
        }
    }
}