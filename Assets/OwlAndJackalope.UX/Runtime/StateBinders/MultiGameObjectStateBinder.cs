using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.StateBinders
{
    public class MultiGameObjectStateBinder : MultiStateBinder<MultiGameObjectStateBinder.GameObjectStateHandler>
    {
        [System.Serializable]
        public class GameObjectStateHandler : StateActionHandler
        {
            [SerializeField]
            private GameObject[] _targets;
            
            protected override void PerformChange(bool isActive)
            {
                foreach (var target in _targets)
                {
                    if (target != null)
                    {
                        target.SetActive(isActive);
                    }
                }
            }
        }
    }
}