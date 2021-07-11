﻿using System;
using OwlAndJackalope.UX.Data.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Data.Extensions
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
            if (detail.ReferenceValue != null && detail.ReferenceValue.Reference != null)
            {
                return detail.ReferenceValue.Reference.ConvertToReference();
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

            return null;
        }
    }
}