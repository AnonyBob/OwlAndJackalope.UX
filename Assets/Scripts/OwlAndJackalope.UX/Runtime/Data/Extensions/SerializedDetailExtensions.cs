using System;
using OwlAndJackalope.UX.Runtime.Data.Serialized;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OwlAndJackalope.UX.Runtime.Data.Extensions
{
    public static class SerializedDetailExtensions
    {
        public static bool GetBool(this BaseSerializedDetail detail)
        {
            return detail.NumericValue > 0;
        }

        public static int GetInt(this BaseSerializedDetail detail)
        {
            return (int)Math.Floor(detail.NumericValue);
        }
        
        public static long GetLong(this BaseSerializedDetail detail)
        {
            return (long)Math.Floor(detail.NumericValue);
        }
        
        public static float GetFloat(this BaseSerializedDetail detail)
        {
            return (float)detail.NumericValue;
        }
        
        public static double GetDouble(this BaseSerializedDetail detail)
        {
            return detail.NumericValue;
        }

        public static T GetEnum<T>(this BaseSerializedDetail detail) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), detail.GetInt());
        }

        public static object GetEnum(this BaseSerializedDetail detail, Type type)
        {
            return Enum.ToObject(type, detail.GetInt());
        }

        public static string GetString(this BaseSerializedDetail detail)
        {
            return detail.StringValue;
        }

        public static IReference GetReference(this BaseSerializedDetail detail)
        {
            var template = detail.ReferenceValue;
            if (template != null && template.Reference != null)
            {
                return template.Reference.ConvertToReference();
            }

            return null;
        }

        public static Vector2 GetVector2(this BaseSerializedDetail detail)
        {
            return detail.VectorValue;
        }
        
        public static Vector3 GetVector3(this BaseSerializedDetail detail)
        {
            return detail.VectorValue;
        }
        
        public static Color GetColor(this BaseSerializedDetail detail)
        {
            return detail.VectorValue;
        }

        public static GameObject GetGameObject(this BaseSerializedDetail detail)
        {
            return detail.GameObjectValue;
        }

        public static Texture2D GetTexture(this BaseSerializedDetail detail)
        {
            return detail.TextureValue;
        }

        public static Sprite GetSprite(this BaseSerializedDetail detail)
        {
            return detail.SpriteValue;
        }

        public static AssetReference GetAssetReference(this BaseSerializedDetail detail)
        {
            return detail.AssetReferenceValue;
        }

        public static TimeSpan GetTimeSpan(this BaseSerializedDetail detail)
        {
            return detail.TimeSpanValue;
        }

        public static object GetValue(this BaseSerializedDetail detail, Type type)
        {
            if (type == typeof(bool))
                return detail.GetBool();
            if (type == typeof(int))
                return detail.GetInt();
            if (type == typeof(long))
                return detail.GetLong();
            if (type == typeof(float))
                return detail.GetFloat();
            if (type == typeof(double))
                return detail.GetDouble();
            if (type == typeof(string))
                return detail.GetString();
            if (type.IsEnum)
                return detail.GetEnum(type);
            if (type == typeof(IReference))
                return detail.GetReference();
            if (type == typeof(Vector2))
                return detail.GetVector2();
            if (type == typeof(Vector3))
                return detail.GetVector3();
            if (type == typeof(Color))
                return detail.GetColor();
            if (type == typeof(GameObject))
                return detail.GetGameObject();
            if (type == typeof(AssetReference))
                return detail.GetAssetReference();
            if (type == typeof(Sprite))
                return detail.GetSprite();
            if (type == typeof(Texture2D))
                return detail.GetTexture();
            if (type == typeof(TimeSpan))
                return detail.GetTimeSpan();
            return null;
        }
    }
}