using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [Serializable]
    public class SerializedIntDetail : SerializedDetail<int>
    {
        
    }
    
    [Serializable]
    public class SerializedLongDetail : SerializedDetail<long>
    {
        
    }
    
    [Serializable]
    public class SerializedFloatDetail : SerializedDetail<float>
    {
        
    }
    
    [Serializable]
    public class SerializedDoubleDetail : SerializedDetail<double>
    {
        
    }
    
    [Serializable]
    public class SerializedStringDetail : SerializedDetail<string>
    {
        
    }

    [Serializable]
    public class SerializedVector2Detail : SerializedDetail<Vector2>
    {
        
    }
    
    [Serializable]
    public class SerializedVector3Detail : SerializedDetail<Vector3>
    {
        
    }
    
    [Serializable]
    public class SerializedColorDetail : SerializedDetail<Color>
    {
        
    }
    
    [Serializable]
    public class SerializedGameObjectDetail : SerializedDetail<GameObject>
    {
        
    }
    
    [Serializable]
    public class SerializedTextureDetail : SerializedDetail<Texture2D>
    {
        
    }
    
    [Serializable]
    public class SerializedSpriteDetail : SerializedDetail<Sprite>
    {
        
    }

    [Serializable]
    public class SerializedReferenceDetail : AbstractSerializedDetail
    {
        [SerializeField]
        public SerializedReferenceTemplate Template;

        private IDetail<IReference> _runtimeDetail;
        private IMutableDetail<IReference> _mutableRuntimeDetail;
        
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