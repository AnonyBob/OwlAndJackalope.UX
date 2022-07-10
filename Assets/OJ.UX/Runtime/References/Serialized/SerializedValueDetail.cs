using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    public interface ISerializedValueDetail<TValue> : ISerializedDetail
    {
    }
    
    [Serializable]
    public abstract class SerializedValueDetail<TValue> : AbstractSerializedDetail, ISerializedValueDetail<TValue>
    {
        [SerializeField]
        public TValue Value;

        private IDetail<TValue> _runtimeDetail;
        private IMutableDetail<TValue> _mutableRuntimeDetail;

        public override Type GetValueType()
        {
            return typeof(TValue);
        }
        
        public override IDetail CreateDetail()
        {
            var detail = new Detail<TValue>(Value);
#if UNITY_EDITOR
            LinkRuntimeDetail(detail);
#endif
            return detail;
        }

        public override void LinkRuntimeDetail(IDetail detail)
        {
            _mutableRuntimeDetail = detail as IMutableDetail<TValue>;
            _runtimeDetail = detail as IDetail<TValue>;
            
            if(_runtimeDetail != null)
            {
                Value = _runtimeDetail.Value;
            }
        }
    }
}