using System;
using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.StateBinders
{
    public abstract class SingleStateBinder : BaseStateBinder
    {
        [SerializeField]
        private StateObserver _state;
        
        [SerializeField]
        private WhenState _is;

        [SerializeField]
        private bool _applyOnStart;

        protected virtual void Start()
        {
            _state.Initialize(_stateModule, HandleActiveChanged, _applyOnStart);
        }

        protected abstract void PerformChange(bool isActive);
        
        private void HandleActiveChanged(bool isActive)
        {
            var invertState = _is == WhenState.NotActive;
            var currentState = isActive == !invertState;
            PerformChange(currentState);
        }

        protected override IEnumerable<StateObserver> GetObservers()
        {
            yield return _state;
        }
    }
}