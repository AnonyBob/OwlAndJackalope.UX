using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Data.Conditions;

namespace OwlAndJackalope.UX.Data.State
{
    public class BaseRuntimeState : IState
    {
        public bool IsActive => _active;
        
        public event Action OnStateActiveChanged;
        
        private readonly IReference _reference;
        private readonly List<ICondition> _conditions;
        private bool _active;

        public BaseRuntimeState(IReference reference, IEnumerable<ICondition> conditions)
        {
            _reference = reference;
            _conditions = conditions.ToList();

            _reference.VersionChanged += HandleVersionChange;
            for (var i = 0; i < _conditions.Count; ++i)
            {
                var condition = _conditions[i];
                foreach (var detailNames in condition.GetUsedDetails())
                {
                    var detail = _reference.GetDetail(detailNames);
                    if (detail != null)
                    {
                        detail.VersionChanged += HandleVersionChange;
                    }
                }
            }

            CheckActive();
        }

        ~BaseRuntimeState()
        {
            _reference.VersionChanged -= HandleVersionChange;
            for (var i = 0; i < _conditions.Count; ++i)
            {
                var condition = _conditions[i];
                foreach (var detailNames in condition.GetUsedDetails())
                {
                    var detail = _reference.GetDetail(detailNames);
                    if (detail != null)
                    {
                        detail.VersionChanged -= HandleVersionChange;
                    }
                }
            }
        }
        
        public void CheckActive()
        {
            var nextActive = true;
            for (var i = 0; i < _conditions.Count; ++i)
            {
                if (!_conditions[i].IsMet(_reference))
                {
                    nextActive = false;
                    break;
                }
            }

            if (_active != nextActive)
            {
                _active = nextActive;
                OnStateActiveChanged?.Invoke();
            }
        }

        private void HandleVersionChange()
        {
            CheckActive();
        }
    }
}