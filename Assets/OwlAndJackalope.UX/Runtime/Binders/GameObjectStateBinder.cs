using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Binders
{
    public class GameObjectStateBinder : SingleStateBinder
    {
        [SerializeField]
        private GameObject[] _targetObjects;

        protected override void PerformChange(bool isActive)
        {
            for (var i = 0; i < _targetObjects.Length; ++i)
            {
                if (_targetObjects[i] != null)
                {
                    _targetObjects[i].SetActive(isActive);
                }
            }
        }
    }
}