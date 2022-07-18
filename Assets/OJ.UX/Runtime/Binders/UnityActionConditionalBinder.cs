using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Binding;
using UnityEngine;
using UnityEngine.Events;

namespace OJ.UX.Runtime.Binders
{
    [System.Serializable]
    public class UnityActionConditionalBinder : ConditionalBinder<UnityActionConditionalBinder.UnityActionConditionalActionDetail>
    {
        [System.Serializable]
        public class UnityActionConditionalActionDetail : IConditionalActionDetail
        {
            [SerializeField]
            public UnityEvent action;

            public void PerformAction()
            {
                action?.Invoke();
            }
        }
    }
}