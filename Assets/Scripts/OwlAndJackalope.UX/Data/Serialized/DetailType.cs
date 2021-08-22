using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OwlAndJackalope.UX.Data.Serialized
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
        public static Type ConvertToType(this DetailType detailType, string enumName, string enumAssembly)
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
                    return GetEnumType(enumName, enumAssembly);
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

        private static Type GetEnumType(string enumName, string assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                return assembly.GetType(enumName);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Can't deserialize enum type: {e}");
            }

            return null;
        }
    }
}