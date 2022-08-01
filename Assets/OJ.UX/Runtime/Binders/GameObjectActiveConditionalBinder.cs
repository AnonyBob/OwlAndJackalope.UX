using System.Collections.Generic;
using OJ.UX.Runtime.Binders.Conditions;
using OJ.UX.Runtime.Utility;
using UnityEngine;

namespace OJ.UX.Runtime.Binders
{
    public class GameObjectActiveConditionalBinder : ConditionalBinder<GameObjectActiveConditionalBinder.GameObjectActiveActionDetail>
    {
        [System.Serializable]
        public struct GameObjectActiveState
        {
            [SerializeField]
            public GameObject GameObject;
            
            [SerializeField]
            public bool ActiveState;
        }
        
        [System.Serializable]
        public class GameObjectActiveActionDetail : IConditionalActionDetail
        {
            [SerializeField, ComplexInlineProperty("GameObject", "@ActiveState")]
            private List<GameObjectActiveState> _gameObjects;
            
            public void PerformAction()
            {
                if (_gameObjects == null)
                    return;

                foreach (var go in _gameObjects)
                {
                    if(go.GameObject != null)
                        go.GameObject.SetActive(go.ActiveState);
                }
            }
        }
    }
}