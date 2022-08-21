using System;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    [System.Serializable]
    public abstract class SerializedConversionValueDetail<TStoredValue, TFinalValue> 
        : AbstractSerializedDetail, ISerializedValueDetail<TFinalValue>
    {
        [SerializeField]
        public TStoredValue Value;
        
        public IDetail<TFinalValue> RuntimeDetail { get; private set; }
        public IMutableDetail<TFinalValue> MutableRuntimeDetail { get; private set; }
        public bool IsProvided { get; private set; }
        public long PreviousRuntimeVersion { get; private set; }

        protected abstract TFinalValue ConvertToFinalValue(TStoredValue value);

        protected abstract TStoredValue ConvertToStoredValue(TFinalValue value);
        
        public override Type GetValueType()
        {
            return typeof(TFinalValue);
        }

        public override IDetail CreateDetail()
        {
            var detail = new Detail<TFinalValue>(ConvertToFinalValue(Value));
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
            MutableRuntimeDetail = detail as IMutableDetail<TFinalValue>;
            RuntimeDetail = detail as IDetail<TFinalValue>;
            
            if(RuntimeDetail != null)
            {
                if (CanMutateRuntimeDetail())
                {
                    Value = ConvertToStoredValue(RuntimeDetail.Value);    
                }
                PreviousRuntimeVersion = RuntimeDetail.Version;
            }
        }

        public override void RespondToChangesInRuntimeDetail()
        {
            if (RuntimeDetail != null && PreviousRuntimeVersion != RuntimeDetail.Version)
            {
                PreviousRuntimeVersion = RuntimeDetail.Version;
                Value =  ConvertToStoredValue(RuntimeDetail.Value);
            }
        }

        public override void ForceUpdateRuntimeDetail()
        {
            if (CanMutateRuntimeDetail())
            {
                MutableRuntimeDetail.Value =  ConvertToFinalValue(Value);
            }
        }

        public override ISerializedDetail Copy()
        {
            var type = GetType();
            var newInstance = (SerializedConversionValueDetail<TStoredValue, TFinalValue>)Activator.CreateInstance(type);
            newInstance.Name = Name;
            newInstance.Value = CopyValue();

            return newInstance;
        }

        protected virtual TStoredValue CopyValue()
        {
            return Value;
        }
    }
}