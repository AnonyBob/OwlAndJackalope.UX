using UnityEngine;
using UnityEngine.Events;

namespace OwlAndJackalope.UX.Runtime.StateBinders
{
    public class UnityActionStateBinder : MultiStateBinder<UnityActionStateBinder.UnityActionStateHandler>
    {
        [System.Serializable]
        public class UnityActionStateHandler : StateActionHandler
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
}