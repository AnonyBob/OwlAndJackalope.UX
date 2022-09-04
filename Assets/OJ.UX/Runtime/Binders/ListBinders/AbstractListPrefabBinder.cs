using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OJ.UX.Runtime.Binding;
using OJ.UX.Runtime.Versions;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    public abstract class AbstractListPrefabBinder<TValue, TPrefab> : AbstractDetailBinder 
        where TPrefab : IInitializableGameObject<TValue>
    {
        [SerializeField]
        private ListObserver<TValue> _list;

        [SerializeField]
        private AbstractPrefabProvider<TValue, TPrefab> _prefabProvider;

        [SerializeField]
        private int _startingSiblingIndex = 0;

        private readonly List<TValue> _removeValues = new List<TValue>(10);
        private readonly Dictionary<TValue, TPrefab> _activePrefabs = new Dictionary<TValue, TPrefab>();
        private Coroutine _waitingForLoadRoutine;

        private void Start()
        {
            _list.Initialize(HandleChange);
            if (!_prefabProvider.IsReady())
            {
                _prefabProvider.Load(this);
            }
        }
        
        private void OnDestroy()
        {
            _list.Destroy();
            _prefabProvider.Unload(this);
        }

        private void HandleChange()
        {
            if (!_prefabProvider.IsReady())
            {
                if (_waitingForLoadRoutine == null)
                {
                    _waitingForLoadRoutine = StartCoroutine(WaitForProviderReady());
                }
                return;
            }
            
            if (!_list.IsSet || _list.Value == null)
            {
                if (_activePrefabs.Count > 0)
                {
                    foreach (var prefab in _activePrefabs)
                    {
                        Destroy(prefab.Value.GameObject);
                    }
                }

                _activePrefabs.Clear();
                return;
            }
            
            //Destroy existing prefabs that are no longer available
            _removeValues.Clear();
            _removeValues.AddRange(_activePrefabs.Keys.Where(x => !_list.Value.Contains(x)));
            foreach (var itemToRemove in _removeValues)
            {
                if (_activePrefabs[itemToRemove].GameObject != null)
                {
                    Destroy(_activePrefabs[itemToRemove].GameObject);    
                }
                _activePrefabs.Remove(itemToRemove);
                
            }
            
            //Create the prefabs that will replace the objects or simply reorder the objects that already exist.
            for(var i = 0; i < _list.Value.Count; ++i)
            {
                var element = _list.Value[i];
                if (!_activePrefabs.TryGetValue(element, out var prefab))
                {
                    prefab = _prefabProvider.CreateElement(element, this);
                    _activePrefabs[element] = prefab;
                }
                
                prefab.GameObject.transform.SetSiblingIndex(i + _startingSiblingIndex);
                prefab.GameObject.name = $"{gameObject.name}-{i}";
            }
        }

        private IEnumerator WaitForProviderReady()
        {
            while (!_prefabProvider.IsReady())
            {
                yield return null;
            }

            _waitingForLoadRoutine = null;
            HandleChange();
        }
    }
}