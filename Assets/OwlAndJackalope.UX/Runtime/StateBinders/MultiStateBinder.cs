using System.Collections.Generic;
using OwlAndJackalope.UX.Runtime.Observers;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.StateBinders
{
    public class MultiStateBinder<TStateActionHandler> : BaseStateBinder 
        where TStateActionHandler : StateActionHandler
    {
        [SerializeField]
        private TStateActionHandler[] _handlers;
        
        protected virtual void Start()
        {
            for (var i = 0; i < _handlers.Length; ++i)
            {
                _handlers[i].InitializeState(_stateModule);
            }
        }
        
        protected override IEnumerable<StateObserver> GetObservers()
        {
            for (var i = 0; i < _handlers.Length; ++i)
            {
                yield return _handlers[i].State;
            }
        }
    }
}