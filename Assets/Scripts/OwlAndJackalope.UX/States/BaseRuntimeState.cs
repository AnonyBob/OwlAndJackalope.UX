using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Conditions;
using OwlAndJackalope.UX.Data;

namespace OwlAndJackalope.UX.States
{
    public class BaseRuntimeState : IState
    {
        public bool IsActive => _active;

        public string Name => _name;
        
        public event Action OnStateActiveChanged;

        private readonly string _name;
        private readonly IReference _reference;
        private readonly List<ICondition> _conditions;
        private bool _active;

        public BaseRuntimeState(string name, IReference reference, IEnumerable<ICondition> conditions)
        {
            _name = name;
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