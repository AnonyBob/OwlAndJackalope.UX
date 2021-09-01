using OwlAndJackalope.UX.Runtime.Modules;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Observers
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

        protected int UpdateDetailName(AbstractDetailObserver target, string previousName, string newName)
        {
            if (target.DetailName == previousName)
            {
                target.DetailName = newName;
                return 1;
            }

            return 0;
        }

        protected abstract int UpdateDetailNames(string previousName, string newName);

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            _referenceModule = _referenceModule != null ? _referenceModule : GetComponentInParent<ReferenceModule>();
            if (_referenceModule == root as ReferenceModule)
            {
                var detailsChanged = UpdateDetailNames(previousName, newName);
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