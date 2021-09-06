using UnityEngine;
using UnityEngine.Events;

namespace OwlAndJackalope.UX.Runtime.Binders
{
    public class UnityActionStateBinder : SingleStateBinder
    {
        [SerializeField]
        private UnityEvent _unityEvent;

        protected override void PerformChange(bool isActive)
        {
            if (isActive)
            {
                _unityEvent?.Invoke();
            }
        }
    }
}