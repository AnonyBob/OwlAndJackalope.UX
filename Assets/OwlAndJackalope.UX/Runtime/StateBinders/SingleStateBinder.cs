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
        private bool _invertState;

        protected virtual void Start()
        {
            _state.Initialize(_stateModule, HandleActiveChanged, false);
        }

        protected abstract void PerformChange(bool isActive);
        
        private void HandleActiveChanged(bool isActive)
        {
            var currentState = isActive == !_invertState;
            PerformChange(currentState);
        }

        protected override IEnumerable<StateObserver> GetObservers()
        {
            yield return _state;
        }
    }
}