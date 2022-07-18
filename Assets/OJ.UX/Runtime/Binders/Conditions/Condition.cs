using System;
using System.Reflection;
using OJ.UX.Runtime.Binding;
using UnityEngine;

namespace OJ.UX.Runtime.Binders.Conditions
{
    [System.Serializable]
    public class Condition : ICondition, IDetailBinder
    {
        [SerializeField] private Observer _observer;

        [SerializeField, SerializeReference] private IConditionComparison _comparison;

        private IConditionChangedHandler _handler;

        public void Initialize(IConditionChangedHandler handler)
        {
            _handler = handler;
            _observer.Initialize(AlertConditionHandler, true);
        }

        public bool IsConditionMet()
        {
            return _comparison.CheckCondition(_observer);
        }

        private void AlertConditionHandler()
        {
            _handler.ConditionHasChanged(this);
        }

        public void Dispose()
        {
            _observer.Dispose();
        }

        public bool RespondToNameChange(ReferenceModule changingModule, string originalName, string newName)
        {
            if (_observer != null)
            {
                var observerType = typeof(Observer);
                var detailNameFieldInfo = observerType.GetField("_detailName", BindingFlags.Instance | BindingFlags.NonPublic);
                var referenceModuleFieldInfo = observerType.GetField("_referenceModule", BindingFlags.Instance | BindingFlags.NonPublic);

                var observerModule = (ReferenceModule)referenceModuleFieldInfo?.GetValue(_observer);
                if (observerModule == changingModule)
                {
                    var detailName = (string)detailNameFieldInfo?.GetValue(_observer);
                    if (detailName == originalName)
                    {
                        detailNameFieldInfo?.SetValue(_observer, newName);
                        return true;
                    }
                }
            }
            
            return false;
        }
}
}