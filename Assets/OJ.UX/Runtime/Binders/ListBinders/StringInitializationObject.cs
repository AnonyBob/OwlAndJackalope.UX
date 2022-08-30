using OJ.UX.Runtime.Versions;
using UnityEngine;
using UnityEngine.Events;

namespace OJ.UX.Runtime.Binders.ListBinders
{
    public class StringInitializationObject : MonoBehaviour, IInitializableGameObject<string>
    {
        public GameObject GameObject => gameObject;
        
        public string Value { get; private set; }

        [SerializeField]
        private UnityEvent _onValueSet = new UnityEvent();
        
        public void Initialize(string value)
        {
            Value = value;
            _onValueSet?.Invoke();
        }
    }
}