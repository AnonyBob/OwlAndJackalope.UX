using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Binders
{
    public abstract class BaseStateBinder : MonoBehaviour, IStateNameChangeHandler
    {
        [SerializeField]
        protected StateModule _stateModule;

        protected virtual void Awake()
        {
            if (_stateModule == null)
            {
                _stateModule = GetComponentInParent<StateModule>();
            }
        }

        protected void OnDestroy()
        {
            foreach (var observer in GetObservers())
            {
                observer?.Dispose();
            }
        }

        protected abstract IEnumerable<StateObserver> GetObservers();

        private int UpdateStateNames(IEnumerable<StateObserver> observers, string previousName, string newName)
        {
            return observers.Sum(x => UpdateStateName(x, previousName, newName));
        }
        
        private int UpdateStateName(StateObserver target, string previousName, string newName)
        {
            if (target.StateName == previousName)
            {
                target.StateName = newName;
                return 1;
            }

            return 0;
        }

        public void HandleStateNameChange(string previousName, string newName, IStateNameChangeHandler root)
        {
            _stateModule = _stateModule != null ? _stateModule : GetComponentInParent<StateModule>();
            if (ReferenceEquals(_stateModule, root))
            {
                var statesChanged = UpdateStateNames(GetObservers(), previousName, newName);
                if (statesChanged > 0)
                {
                    Debug.Log($"{name} updated {statesChanged} observers to {newName}");
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }
            }
        }
    }
}