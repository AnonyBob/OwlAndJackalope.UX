using System;
using System.Reflection;
using OwlAndJackalope.UX.Runtime.Data.Serialized.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OwlAndJackalope.UX.Runtime.Data.Serialized
{
    /// <summary>
    /// Serialized detail type indicates the data type that this will represent.
    /// It can be a primitive type or another reference object.
    /// </summary>
    public enum DetailType
    {
        Bool = 0,
        Integer = 1,
        Long = 2,
        Float = 3,
        Double = 4,
        Enum = 5,
        String = 6,
        Reference = 7,
        Vector2 = 8,
        Vector3 = 9,
        Color = 10,
        GameObject = 11,
        Texture = 12,
        Sprite = 13,
        AssetReference = 14,
        TimeSpan = 15,
    }

    public static class DetailTypeExtensions
    {
        public static Type ConvertToType(this DetailType detailType, int enumId)
        {
            switch (detailType)
            {
                case DetailType.Bool:
                    return typeof(bool);
                case DetailType.Integer:
                    return typeof(int);
                case DetailType.Long:
                    return typeof(long);
                case DetailType.Float:
                    return typeof(float);
                case DetailType.Double:
                    return typeof(double);
                case DetailType.Enum:
                    return SerializedDetailEnumCache.GetCreator(enumId)?.EnumType;
                case DetailType.String:
                    return typeof(string);
                case DetailType.Reference:
                    return typeof(IReference);
                case DetailType.Vector2:
                    return typeof(Vector2);
                case DetailType.Vector3:
                    return typeof(Vector3);
                case DetailType.Color:
                    return typeof(Color);
                case DetailType.GameObject:
                    return typeof(GameObject);
                case DetailType.AssetReference:
                    return typeof(AssetReference);
                case DetailType.Sprite:
                    return typeof(Sprite);
                case DetailType.Texture:
                    return typeof(Texture2D);
                case DetailType.TimeSpan:
                    return typeof(TimeSpan);
                default:
                    throw new ArgumentOutOfRangeException(nameof(detailType), detailType, null);
            }
        }
        
        public static DetailType ConvertToEnum(this Type type)
        {
            if (type == typeof(bool))
            {
                return DetailType.Bool;
            }
            if (type == typeof(int))
            {
                return DetailType.Integer;
            }
            if (type == typeof(long))
            {
                return DetailType.Long;
            }
            if (type == typeof(float))
            {
                return DetailType.Float;
            }
            if (type == typeof(double))
            {
                return DetailType.Double;
            }
            if (type == typeof(string))
            {
                return DetailType.String;
            }
            if (typeof(IReference).IsAssignableFrom(type))
            {
                return DetailType.Reference;
            }
            if (type == typeof(Vector2))
            {
                return DetailType.Vector2;
            }
            if (type == typeof(Vector3))
            {
                return DetailType.Vector3;
            }
            if (type == typeof(Color))
            {
                return DetailType.Color;
            }
            if (type == typeof(GameObject))
            {
                return DetailType.GameObject;
            }
            if (type == typeof(Texture2D))
            {
                return DetailType.Texture;
            }
            if (type == typeof(Sprite))
            {
                return DetailType.Sprite;
            }
            if (type == typeof(TimeSpan))
            {
                return DetailType.TimeSpan;
            }

            if (type.IsEnum)
            {
                return DetailType.Enum;
            }

            Debug.LogWarning($"{type} does not have a defined DetailType");
            return DetailType.Bool;
        }

        public static bool IsComparable(this DetailType type)
        {
            switch (type)
            {
                case DetailType.Bool:
                case DetailType.Integer:
                case DetailType.Long:
                case DetailType.Float:
                case DetailType.Double:
                case DetailType.Enum:
                case DetailType.String:
                case DetailType.TimeSpan:
                    return true; 
                default:
                    return false;
            }
        }
    }
}