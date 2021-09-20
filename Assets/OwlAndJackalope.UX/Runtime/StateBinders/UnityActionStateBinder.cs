using UnityEngine;
using UnityEngine.Events;

namespace OwlAndJackalope.UX.Runtime.StateBinders
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