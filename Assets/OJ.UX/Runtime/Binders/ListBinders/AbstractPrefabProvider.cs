using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    public abstract class AbstractPrefabProvider<TValue, TPrefab> : ScriptableObject where TPrefab : IInitializableGameObject<TValue>
    {
        /// <summary>
        /// Becomes true whenever the assets provided by this are available.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsReady();

        /// <summary>
        /// Call this whenever you want to start loading. Keep track of the parent that asked for the loading.
        /// </summary>
        public abstract void Load(MonoBehaviour parent);

        /// <summary>
        /// Call this whenever you want to stop loading elements for the given parent. This can be used to unload
        /// assets or keep track of how many usages are still out there.
        /// </summary>
        public abstract void Unload(MonoBehaviour parent);
        
        /// <summary>
        /// Creates an element for the value. It will use the parent for instantiation if needed.
        /// </summary>
        public abstract TPrefab CreateElement(TValue value, MonoBehaviour parent);
    }
}