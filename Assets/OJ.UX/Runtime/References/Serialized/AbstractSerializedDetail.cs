using System;
using System.Collections.Generic;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    public interface ISerializedDetail
    {
        string GetName();
        
        IDetail CreateDetail();

        void LinkRuntimeDetail(IDetail detail);
    }

    [Serializable]
    public abstract class AbstractSerializedDetail : ISerializedDetail
    {
        [SerializeField]
        public string Name;

        public string GetName()
        {
            return Name;
        }

        public abstract IDetail CreateDetail();

        public abstract void LinkRuntimeDetail(IDetail detail);
    }
    
    [Serializable]
    public class SerializedDetail<TValue> : AbstractSerializedDetail
    {
        [SerializeField]
        public TValue Value;

        private IDetail<TValue> _runtimeDetail;
        private IMutableDetail<TValue> _mutableRuntimeDetail;

        public string GetName()
        {
            return Name;
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