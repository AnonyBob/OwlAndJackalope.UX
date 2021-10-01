using System;
using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Conditions;
using OwlAndJackalope.UX.Runtime.Data;

namespace OwlAndJackalope.UX.Runtime.States
{
    public class BaseRuntimeState : IState
    {
        public bool IsActive => _active;

        public string Name => _name;
        
        public event Action OnStateActiveChanged;

        private readonly string _name;
        private readonly IReference _reference;
        private readonly List<ICondition> _conditions;
        private readonly List<IDetail> _usedDetails = new List<IDetail>();
        
        private bool _active;

        public BaseRuntimeState(string name, IReference reference, IEnumerable<ICondition> conditions)
        {
            _name = name;
            _reference = reference;
            _conditions = conditions?.ToList() ?? new List<ICondition>();

            _reference.VersionChanged += HandleReferenceVersionChange;
            RegisterDetails();
            CheckActive();
        }

        ~BaseRuntimeState()
        {
            _reference.VersionChanged -= HandleReferenceVersionChange;
            ClearDetails();
        }
        
        private void CheckActive()
        {
            var nextActive = false;
            for (var i = 0; i < _conditions.Count; ++i)
            {
                if (_conditions[i].IsMet(_reference))
                {
                    nextActive = true;
                    break;
                }
            }

            if (_active != nextActive)
            {
                _active = nextActive;
                OnStateActiveChanged?.Invoke();
            }
        }

        private void HandleReferenceVersionChange()
        {
            ClearDetails();
            RegisterDetails();
            CheckActive();
        }

        private void HandleDetailVersionChange()
        {
            CheckActive();
        }

        private void ClearDetails()
        {
            for(var i = _usedDetails.Count - 1; i > -1; --i)
            {
                _usedDetails[i].VersionChanged -= HandleDetailVersionChange;
                _usedDetails.RemoveAt(i);
            }
        }

        private void RegisterDetails()
        {
            for (var i = 0; i < _conditions.Count; ++i)
            {
                var condition = _conditions[i];
                foreach (var detailNames in condition.GetUsedDetails())
                {
                    var detail = _reference.GetDetail(detailNames);
                    if (detail != null)
                    {
                        detail.VersionChanged += HandleDetailVersionChange;
                        _usedDetails.Add(detail);
                    }
                }
            }
        }
    }
}