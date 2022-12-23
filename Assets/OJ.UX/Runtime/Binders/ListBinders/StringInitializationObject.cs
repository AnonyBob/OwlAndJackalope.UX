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
        private UnityEvent<string> _onValueSet = new UnityEvent<string>();
        
        public void Initialize(string value)
        {
            Value = value;
            _onValueSet?.Invoke(Value);

            
            var alertables = GetComponents<IAlertable>();
            foreach (var alertable in alertables)
            {
                alertable.AlertOfChange();
            }
        }
    }
}