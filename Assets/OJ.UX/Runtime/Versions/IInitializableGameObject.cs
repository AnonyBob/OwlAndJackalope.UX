using UnityEngine;

namespace OJ.UX.Runtime.Versions
{
    public interface IInitializableGameObject<in TValue>
    {
        GameObject GameObject { get; }
        
        void Initialize(TValue value);
    }
}