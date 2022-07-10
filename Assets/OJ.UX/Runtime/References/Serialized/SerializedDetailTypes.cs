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

    [Serializable, SerializedDetailDisplay("Reference", "Special")]
    public sealed class SerializedReferenceDetail : AbstractSerializedDetail, ISerializedValueDetail<IReference>
    {
        [SerializeField]
        public SerializedReferenceTemplate Value;

        public IDetail<IReference> RuntimeDetail { get; private set; }
        public IMutableDetail<IReference> MutableRuntimeDetail { get; private set; }
        public bool IsProvided { get; private set; }
        
        public override Type GetValueType()
        {
            return typeof(IReference);
        }
        
        public override IDetail CreateDetail()
        {
            var detail = new Detail<IReference>(Value.CreateReference());
#if UNITY_EDITOR
            LinkRuntimeDetail(detail, false);
#endif
            return detail;
        }

        public override bool CanMutateRuntimeDetail() => false;

        public override bool IsRuntimeDetailProvided() => IsProvided;

        public override void LinkRuntimeDetail(IDetail detail, bool isProvided)
        {
            RuntimeDetail = detail as IDetail<IReference>;
            MutableRuntimeDetail = detail as IMutableDetail<IReference>;
            IsProvided = isProvided;
        }

        public override void RespondToChangesInRuntimeDetail()
        {
            //Do nothing right now.
        }
        
        public override void ForceUpdateRuntimeDetail()
        {
            throw new NotImplementedException();
        }
    }
}