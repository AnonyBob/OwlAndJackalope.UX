using System;
using OwlAndJackalope.UX.Runtime.Modules;
using OwlAndJackalope.UX.Runtime.States;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Observers
{
    [System.Serializable]
    public class StateObserver : IDisposable
    {
        public IState State { get; private set; }

        public bool IsActive => State?.IsActive ?? false;
        
        public bool IsSet => State != null;

        public event Action<bool> OnActiveChange;
        
        [SerializeField]
        public string StateName;

        public void Initialize(StateModule stateModule, Action<bool> activeHandler = null, bool suppressInitial = true)
        {
            State = stateModule.GetState(StateName);
            if (activeHandler != null)
            {
                OnActiveChange += activeHandler;    
            }
            
            if (State != null)
            {
                State.OnStateActiveChanged += HandleActiveChanged;
                if (!suppressInitial)
                {
                    HandleActiveChanged();    
                }
            }
        }
        
        private void HandleActiveChanged()
        {
            OnActiveChange?.Invoke(IsActive);
        }
        
        public void Dispose()
        {
            if (State != null)
            {
                State.OnStateActiveChanged -= HandleActiveChanged;
            }
        }
    }
}