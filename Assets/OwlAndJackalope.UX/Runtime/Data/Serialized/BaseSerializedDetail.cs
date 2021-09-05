using System;
using System.Reflection;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// Multi-purpose serialized detail. It can map to various primitive types or point to another reference.
    /// It determines which type it should serialize to based on the set detail type. When required it will
    /// be converted to the appropriate detail.
    /// </summary>
    [System.Serializable]
    public class BaseSerializedDetail : ISerializedDetail
    {
        public string Name => _name;
        public Type Type => _type.ConvertToType(_enumId);
        public double NumericValue => _value;
        public string StringValue => _stringValue;
        public ReferenceTemplate ReferenceValue => _referenceValue as ReferenceTemplate;
        public Vector4 VectorValue => _vectorValue;
        public GameObject GameObjectValue => _referenceValue as GameObject;
        public AssetReference AssetReferenceValue => _assetReferenceValue;
        public Texture2D TextureValue => _referenceValue as Texture2D;
        public Sprite SpriteValue => _referenceValue as Sprite;
        public TimeSpan TimeSpanValue => TimeSpan.FromTicks((long) Math.Floor(_value));
        
        //Identifying information used to select for the detail.
        [SerializeField] protected string _name;
        [SerializeField] protected DetailType _type;
        [SerializeField] protected int _enumId = 0;

        //Stored information representing the value stored in the detail at start, this will be
        //overwritten if a reference provider is being used.
        [SerializeField] protected double _value = 0;
        [SerializeField] protected string _stringValue;
        [SerializeField] protected UnityEngine.Object _referenceValue;
        [SerializeField] protected Vector4 _vectorValue;
        [SerializeField] protected AssetReference _assetReferenceValue;

        public IDetail ConvertToDetail()
        {
            switch (_type)
            {
                case DetailType.Bool:
                    return new BaseDetail<bool>(_name, this.GetBool());
                case DetailType.Integer:
                    return new BaseDetail<int>(_name, this.GetInt());
                case DetailType.Long:
                    return new BaseDetail<long>(_name, this.GetLong());
                case DetailType.Float:
                    return new BaseDetail<float>(_name, this.GetFloat());
                case DetailType.Double:
                    return new BaseDetail<double>(_name, this.GetDouble());
                case DetailType.String:
                    return new BaseDetail<string>(_name, this.GetString());
                case DetailType.Reference:
                    return CreateReferenceDetail();
                case DetailType.Enum:
                    return CreateEnumDetail();
                case DetailType.Vector2:
                    return new BaseDetail<Vector2>(_name, this.GetVector2());
                case DetailType.Vector3:
                    return new BaseDetail<Vector3>(_name, this.GetVector3());
                case DetailType.Color:
                    return new BaseDetail<Color>(_name, this.GetColor());
                case DetailType.GameObject:
                    return new BaseDetail<GameObject>(_name, this.GetGameObject());
                case DetailType.Texture:
                    return new BaseDetail<Texture2D>(_name, this.GetTexture());
                case DetailType.Sprite:
                    return new BaseDetail<Sprite>(_name, this.GetSprite());
                case DetailType.AssetReference:
                    return new BaseDetail<AssetReference>(_name, this.GetAssetReference());
                case DetailType.TimeSpan:
                    return new BaseDetail<TimeSpan>(_name, this.GetTimeSpan());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IDetail CreateEnumDetail()
        {
            var creator = SerializedDetailEnumCache.GetCreator(_enumId);
            if (creator == null)
            {
                Debug.LogWarning($"{_enumId} did not have a creator registered");
                return null;
            }

            return creator.CreateDetail(_name, this.GetInt());
        }

        private IDetail CreateReferenceDetail()
        {
            if (ReferenceValue != null)
            {
                return new BaseDetail<IReference>(_name, this.GetReference());
            } 
            return new BaseDetail<IReference>(_name, null);
        }
    }
}