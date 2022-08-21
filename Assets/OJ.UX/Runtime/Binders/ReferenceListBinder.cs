using System.Collections.Generic;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.References;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public abstract class ReferencePrefabProvider : ScriptableObject
    {
        public abstract PrefabDetail GetPrefab(IReference reference, MonoBehaviour parent);
    }

    public enum PrefabLoadedState
    {
        Loading = 0,
        Loaded = 1,
        Failed = 2,
        Destroyed = 3,
    }
    
    public class PrefabDetail
    {
        public ReferenceModule PlaceHolderObject;
        public ReferenceModule LoadedObject;
        public PrefabLoadedState State;
        public Coroutine LoadingRoutine;

        public void OnLoadComplete(ReferenceModule loadedObject)
        {
            LoadingRoutine = null;
        }
        
        public void Destroy(MonoBehaviour parent)
        {
            if (LoadingRoutine != null)
            {
                parent.StopCoroutine(LoadingRoutine);
            }
            
            if(PlaceHolderObject != null)
                Destroy(PlaceHolderObject);
            
            if(LoadedObject != null)
                Destroy(LoadedObject);

            State = PrefabLoadedState.Destroyed;
        }
    }
    
    public class ReferenceListBinder : AbstractDetailBinder
    {
        [SerializeField]
        private ListObserver<IReference> _list;

        [SerializeField]
        private ReferencePrefabProvider _prefabProvider;

        private readonly Dictionary<IReference, PrefabDetail> _activePrefabs = new Dictionary<IReference, PrefabDetail>();

        private void Start()
        {
            _list.Initialize(HandleChange);
        }

        private void HandleChange()
        {
            if (!_list.IsSet || _list.Value == null)
            {
                if (_activePrefabs.Count > 0)
                {
                    foreach (var prefab in _activePrefabs)
                    {
                        prefab.Value.Destroy(this);
                    }
                }

                _activePrefabs.Clear();
            }
            
            //Destroy existing prefabs that are no longer available
            
            //Create the new prefabs that will replace them.
        }

        private void OnDestroy()
        {
            _list.Destroy();
        }
    }
}