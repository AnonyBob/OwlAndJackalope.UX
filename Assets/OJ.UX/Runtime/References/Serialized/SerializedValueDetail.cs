using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    public interface ISerializedValueDetail<TValue> : ISerializedDetail
    {
        public IDetail<TValue> RuntimeDetail { get; }
        public IMutableDetail<TValue> MutableRuntimeDetail { get; }
        public bool IsProvided { get; }
    }
    
    [Serializable]
    public abstract class SerializedValueDetail<TValue> : AbstractSerializedDetail, ISerializedValueDetail<TValue>
    {
        [SerializeField]
        public TValue Value;

        public IDetail<TValue> RuntimeDetail { get; private set; }
        public IMutableDetail<TValue> MutableRuntimeDetail { get; private set; }
        public bool IsProvided { get; private set; }
        public long PreviousRuntimeVersion { get; private set; }

        public override Type GetValueType()
        {
            return typeof(TValue);
        }
        
        public override IDetail CreateDetail()
        {
            var detail = new Detail<TValue>(Value);
#if UNITY_EDITOR
            LinkRuntimeDetail(detail, false);
#endif
            return detail;
        }

        public override bool CanMutateRuntimeDetail() => MutableRuntimeDetail != null;

        public override bool IsRuntimeDetailProvided() => IsProvided;

        public override void LinkRuntimeDetail(IDetail detail, bool isProvided)
        {
            IsProvided = isProvided;
            MutableRuntimeDetail = detail as IMutableDetail<TValue>;
            RuntimeDetail = detail as IDetail<TValue>;
            
            if(RuntimeDetail != null)
            {
                Value = RuntimeDetail.Value;
                PreviousRuntimeVersion = RuntimeDetail.Version;
            }
        }

        public override void RespondToChangesInRuntimeDetail()
        {
            if (RuntimeDetail != null && PreviousRuntimeVersion != RuntimeDetail.Version)
            {
                PreviousRuntimeVersion = RuntimeDetail.Version;
                Value = RuntimeDetail.Value;
            }
        }

        public override void ForceUpdateRuntimeDetail()
        {
            if (CanMutateRuntimeDetail())
            {
                MutableRuntimeDetail.Value = Value;
            }
        }
    }
}