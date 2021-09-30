using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// Serializes a collection detail by simply serializing a collection of serialized details.
    /// </summary>
    [System.Serializable]
    public class BaseSerializedCollectionDetail : ISerializedDetail
    {
        public string Name => _name;
        public Type Type => typeof(List<>).MakeGenericType(_type.ConvertToType(_enumId));
        
        [SerializeField] protected string _name;
        [SerializeField] protected DetailType _type;
        [SerializeField] protected int _enumId;
        
        [SerializeField] private List<BaseSerializedDetail> _collection = new List<BaseSerializedDetail>();

        public BaseSerializedCollectionDetail(string name, Type itemType)
        {
            _name = name;
            var collectionItemType = itemType;
            _type = collectionItemType.ConvertToEnum();
            if (_type == DetailType.Enum)
            {
                _enumId = SerializedDetailEnumCache.GetCreator(collectionItemType.Name).EnumId;
            }
        }
        
        public IDetail ConvertToDetail()
        {
            switch (_type)
            {
                case DetailType.Bool:
                    return new BaseCollectionDetail<bool>(_name, CreateList(x => x.GetBool()), false);
                case DetailType.Integer:
                    return new BaseCollectionDetail<int>(_name, CreateList(x => x.GetInt()), false);
                case DetailType.Long:
                    return new BaseCollectionDetail<long>(_name, CreateList(x => x.GetLong()), false);
                case DetailType.Float:
                    return new BaseCollectionDetail<float>(_name, CreateList(x => x.GetFloat()), false);
                case DetailType.Double:
                    return new BaseCollectionDetail<double>(_name, CreateList(x => x.GetDouble()), false);
                case DetailType.Enum:
                    return CreateEnumList();
                case DetailType.String:
                    return new BaseCollectionDetail<string>(_name, CreateList(x => x.GetString()), false);
                case DetailType.Reference:
                    return new BaseCollectionDetail<IReference>(_name, CreateList(x => x.GetReference()), false);
                case DetailType.Vector2:
                    return new BaseCollectionDetail<Vector2>(_name, CreateList(x => x.GetVector2()), false);
                case DetailType.Vector3:
                    return new BaseCollectionDetail<Vector3>(_name, CreateList(x => x.GetVector3()), false);
                case DetailType.Color:
                    return new BaseCollectionDetail<Color>(_name, CreateList(x => x.GetColor()), false);
                case DetailType.GameObject:
                    return new BaseCollectionDetail<GameObject>(_name, CreateList(x => x.GetGameObject()), false);
                case DetailType.Texture:
                    return new BaseCollectionDetail<Texture2D>(_name, CreateList(x => x.GetTexture()), false);
                case DetailType.Sprite:
                    return new BaseCollectionDetail<Sprite>(_name, CreateList(x => x.GetSprite()), false);
#if USE_ADDRESSABLES
                case DetailType.AssetReference:
                    return new BaseCollectionDetail<UnityEngine.AddressableAssets.AssetReference>(_name, 
                        CreateList(x => x.GetAssetReference()), false);
#endif
                case DetailType.TimeSpan:
                    return new BaseCollectionDetail<TimeSpan>(_name, CreateList(x => x.GetTimeSpan()), false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<TValue> CreateList<TValue>(Func<BaseSerializedDetail, TValue> getValue)
        {
            var list = _collection.Select(getValue).ToList();
            return list;
        }

        private IDetail CreateEnumList()
        {
            var creator = SerializedDetailEnumCache.GetCreator(_enumId);
            if (creator == null)
            {
                Debug.LogError($"{_enumId} is not a defined enum type");
                return null;
            }

            return creator.CreateCollectionDetail(_name, _collection.Select(x => x.GetInt()));
        }
    }
}