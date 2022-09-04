using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable]
    public class SerializedBoolDetail : SerializedValueDetail<bool>
    {
        
    }
    
    [Serializable, SerializedDetailDisplay("Integer")]
    public class SerializedIntDetail : SerializedValueDetail<int>
    {
        
    }
    
    [Serializable, SerializedDetailDisplay("Long")]
    public class SerializedLongDetail : SerializedValueDetail<long>
    {
        
    }
    
    [Serializable, SerializedDetailDisplay("Float")]
    public class SerializedFloatDetail : SerializedValueDetail<float>
    {
        
    }
    
    [Serializable]
    public class SerializedDoubleDetail : SerializedValueDetail<double>
    {
        
    }
    
    [Serializable]
    public class SerializedStringDetail : SerializedValueDetail<string>
    {
        
    }

    [Serializable]
    public class SerializedVector2Detail : SerializedValueDetail<Vector2>
    {
        
    }
    
    [Serializable]
    public class SerializedVector3Detail : SerializedValueDetail<Vector3>
    {
        
    }
    
    [Serializable]
    public class SerializedVector2IntDetail : SerializedValueDetail<Vector2Int>
    {
        
    }
    
    [Serializable]
    public class SerializedVector3IntDetail : SerializedValueDetail<Vector3Int>
    {
        
    }
    
    [Serializable]
    public class SerializedColorDetail : SerializedValueDetail<Color>
    {
        
    }
    
    [Serializable]
    public class SerializedGameObjectDetail : SerializedValueDetail<GameObject>
    {
        
    }
    
    [Serializable]
    public class SerializedTextureDetail : SerializedValueDetail<Texture2D>
    {
        
    }
    
    [Serializable]
    public class SerializedSpriteDetail : SerializedValueDetail<Sprite>
    {
        
    }

    [Serializable, SerializedDetailDisplay("EyeColor", "Enums")]
    public class SerializedEyeColorDetail : SerializedValueDetail<EyeColor>
    {
        
    }
}