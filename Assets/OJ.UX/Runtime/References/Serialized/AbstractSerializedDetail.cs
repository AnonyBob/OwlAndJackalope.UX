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
}