using System.Collections.Generic;
using OJ.UX.Runtime.Binders.Conditions;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class GameObjectActiveConditionalBinder : ConditionalBinder<GameObjectActiveConditionalBinder.GameObjectActiveActionDetail>
    {
        [System.Serializable]
        public class GameObjectActiveActionDetail : IConditionalActionDetail
        {
            [SerializeField]
            private bool _setActive;
            
            [SerializeField]
            private List<GameObject> _gameObjects;
            
            public void PerformAction()
            {
                if (_gameObjects == null)
                    return;

                foreach (var go in _gameObjects)
                {
                    if(go != null)
                        go.SetActive(_setActive);
                }
            }
        }
    }
}