using System;
using System.Collections.Generic;
using UnityEngine;

namespace OJ.UX.Runtime.References.Serialized
{
    public interface ISerializedDetail
    {
        string GetName();

        Type GetValueType();
        
        IDetail CreateDetail();

        bool CanMutateRuntimeDetail();

        bool IsRuntimeDetailProvided();
        
        void LinkRuntimeDetail(IDetail detail, bool isProvided);

        void RespondToChangesInRuntimeDetail();
        
        void ForceUpdateRuntimeDetail();
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

        public abstract Type GetValueType();

        public abstract IDetail CreateDetail();

        public abstract bool CanMutateRuntimeDetail();

        public abstract bool IsRuntimeDetailProvided();

        public abstract void LinkRuntimeDetail(IDetail detail, bool isProvided);

        public abstract void RespondToChangesInRuntimeDetail();

        public abstract void ForceUpdateRuntimeDetail();
    }
}