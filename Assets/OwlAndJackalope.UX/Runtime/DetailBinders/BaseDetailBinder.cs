using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.DetailBinders
{
    /// <summary>
    /// Simple binder monobehavior that will bind details to certain in game actions.
    /// </summary>
    public abstract class BaseDetailBinder : MonoBehaviour, IDetailNameChangeHandler
    {
        [SerializeField]
        protected ReferenceModule _referenceModule;
        
        protected virtual void Awake()
        {
            if (_referenceModule == null)
            {
                _referenceModule = GetComponentInParent<ReferenceModule>();
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var observer in GetDetailObservers())
            {
                observer?.Dispose();
            }
        }

        protected abstract IEnumerable<AbstractDetailObserver> GetDetailObservers();

        private int UpdateDetailNames(IEnumerable<AbstractDetailObserver> observers, string previousName,
            string newName)
        {
            return observers.Sum(observer => UpdateDetailName(observer, previousName, newName));
        }
        
        private int UpdateDetailName(AbstractDetailObserver target, string previousName, string newName)
        {
            if (target != null && target.DetailName == previousName)
            {
                target.DetailName = newName;
                return 1;
            }

            return 0;
        }
        
        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            _referenceModule = _referenceModule != null ? _referenceModule : GetComponentInParent<ReferenceModule>();
            if (ReferenceEquals(_referenceModule, root))
            {
                var detailsChanged = UpdateDetailNames(GetDetailObservers(), previousName, newName);
                if (detailsChanged > 0)
                {
                    Debug.Log($"{name} updated {detailsChanged} observers to {newName}");
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }
            }

        }
    }
}