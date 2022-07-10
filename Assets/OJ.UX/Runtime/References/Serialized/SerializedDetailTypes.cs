﻿using System;
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

    [Serializable, SerializedDetailDisplay("Reference")]
    public class SerializedReferenceDetail : AbstractSerializedDetail, ISerializedValueDetail<IReference>
    {
        [SerializeField]
        public SerializedReferenceTemplate Template;

        private IDetail<IReference> _runtimeDetail;
        private IMutableDetail<IReference> _mutableRuntimeDetail;
        
        public Type GetDataType()
        {
            return typeof(IReference);
        }
        
        public override IDetail CreateDetail()
        {
            var detail = new Detail<IReference>(Template.CreateReference());
#if UNITY_EDITOR
            LinkRuntimeDetail(detail);
#endif
            return detail;
        }

        public override void LinkRuntimeDetail(IDetail detail)
        {
            _runtimeDetail = detail as IDetail<IReference>;
            _mutableRuntimeDetail = detail as IMutableDetail<IReference>;
        }
    }
}