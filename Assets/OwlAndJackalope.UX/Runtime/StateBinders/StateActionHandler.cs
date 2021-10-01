using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.StateBinders
{
    public abstract class StateActionHandler
    {
        [SerializeField] 
        public StateObserver State;

        [SerializeField]
        public WhenState Is;

        [SerializeField]
        public bool ApplyOnStart;
        
        protected abstract void PerformChange(bool currentState);

        protected virtual void ApplyInitialChange()
        {
            //Do nothing by default.
        }

        public void InitializeState(StateModule module)
        {
            State.Initialize(module, HandleChange, !ApplyOnStart);
            ApplyInitialChange();
        }
        
        private void HandleChange(bool isActive)
        {
            var invertState = Is == WhenState.NotActive;
            var currentState = isActive == !invertState;
            PerformChange(currentState);
        }
    }
}