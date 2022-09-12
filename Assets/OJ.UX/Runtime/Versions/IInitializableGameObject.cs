using UnityEngine;

namespace OJ.UX.Runtime.Versions
{
    public interface IInitializableGameObject<TValue>
    {
        GameObject GameObject { get; }
        
        TValue Value { get; }
        
        void Initialize(TValue value);
    }
}