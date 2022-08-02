using System;
using System.Collections.Generic;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [System.Serializable]
    public abstract class SerializedListValueDetail<T> : SerializedValueDetail<List<T>>
    {
        public override IDetail CreateDetail()
        {
            var detail = new ListDetail<T>(Value);
#if UNITY_EDITOR
            LinkRuntimeDetail(detail, false);
#endif
            return detail;
        }

        protected override List<T> CopyValue()
        {
            return new List<T>(Value);
        }
    }

    [System.Serializable, SerializedDetailDisplay("Integer[]", "Lists")]
    public class SerializedIntListValueDetail : SerializedListValueDetail<int>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Long[]", "Lists")]
    public class SerializedLongListValueDetail : SerializedListValueDetail<long>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Float[]", "Lists")]
    public class SerializedFloatListValueDetail : SerializedListValueDetail<float>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Double[]", "Lists")]
    public class SerializedDoubleListValueDetail : SerializedListValueDetail<double>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("String[]", "Lists")]
    public class SerializedStringListValueDetail : SerializedListValueDetail<string>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Bool[]", "Lists")]
    public class SerializedBoolListValueDetail : SerializedListValueDetail<bool>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("GameObject[]", "Lists")]
    public class SerializedGameObjectListValueDetail : SerializedListValueDetail<GameObject>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Texture[]", "Lists")]
    public class SerializedTextureListValueDetail : SerializedListValueDetail<Texture>
    {
        
    }
    
    [System.Serializable, SerializedDetailDisplay("Sprite[]", "Lists")]
    public class SerializedSpriteListValueDetail : SerializedListValueDetail<Sprite>
    {
        
    }
}